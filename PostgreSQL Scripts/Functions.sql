CREATE FUNCTION update_client_status()
	RETURNS TRIGGER AS $$
	DECLARE
    	total_spent NUMERIC;
        client_id INT;
	BEGIN
        IF TG_OP = 'DELETE' THEN
            client_id = OLD."ClientId";
        ELSE
            client_id = NEW."ClientId";
        END IF;

		SELECT COALESCE(SUM("TotalPrice"), 0) INTO total_spent
		FROM "Checks"
		WHERE "ClientId" = client_id;

		UPDATE "Clients"
		SET "Status" = CASE
			WHEN total_spent > 300000 THEN 2
			WHEN total_spent > 100000 THEN 1
			ELSE 0
		END,
		"MoneySpent" = total_spent
		WHERE "Id" = client_id;
		
		RETURN NULL;  
	END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_access_new_material()
    RETURNS TRIGGER AS $$
    BEGIN
        IF NEW."IsCertifiedMaterial" = TRUE AND 
           (TG_OP = 'INSERT' OR OLD."IsCertifiedMaterial" IS DISTINCT FROM TRUE) THEN
            
            INSERT INTO "DoctorMaterialAccesses" ("EmployeeId", "MaterialId")
            SELECT "Id", NEW."Id"
            FROM "Employees"
            WHERE "IsCertified" = TRUE
            ON CONFLICT DO NOTHING; --при конфликте ничего не делаем
        END IF;
        
        IF NEW."IsCertifiedMaterial" = FALSE AND 
           TG_OP = 'UPDATE' AND 
           OLD."IsCertifiedMaterial" = TRUE THEN
            
            DELETE FROM "DoctorMaterialAccesses"
            WHERE "MaterialId" = NEW."Id";
        END IF;
        
        RETURN NEW;
    END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_bonuses_for_client()
	RETURNS TRIGGER AS $$
	DECLARE
		client_status INT;
        bonus_percent FLOAT;
	BEGIN		
		SELECT "Status" INTO client_status 
        	FROM "Clients" 
        	WHERE "Id" = NEW."ClientId";
		
		IF client_status = 0 THEN 
            bonus_percent := 0.05;
        ELSIF client_status = 1 THEN
            bonus_percent := 0.1;
        ELSE 
            bonus_percent := 0.2;
        END IF;
		
		INSERT INTO "Bonuses" ("AddedAt", "ExpiredAt", "Amount", "ClientId")
		VALUES (
			NOW(), 
			NOW() + INTERVAL '1 year',
			FLOOR(NEW."TotalPrice" * bonus_percent)::INT, 
			NEW."ClientId"
        );
		UPDATE "Clients"
	    SET "MoneySpent" = "MoneySpent" + NEW."TotalPrice" 
	    WHERE "Id" = NEW."ClientId";
		RETURN NEW;
	END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE expire_old_bonuses_procedure() -- по расписанию должно вызываться (pg_cron)
	LANGUAGE plpgsql AS $$
	BEGIN
		DELETE FROM "Bonuses" 
		WHERE "ExpiredAt" < CURRENT_TIMESTAMP OR
			("Amount" = 0);
	END;
$$;

-- Исправленная функция get_booking_statistic (убраны лишние точки с запятой, согласованы алиасы, добавлена проверка NULL)
CREATE OR REPLACE FUNCTION get_booking_statistic(
    start_date DATE,
    end_date DATE
) RETURNS TABLE(
    clinic_id INT,
    clinic_location VARCHAR,
    total_bookings BIGINT,           
    completed_bookings BIGINT,       
    cancelled_bookings BIGINT,      
    pending_bookings BIGINT,
    total_revenue NUMERIC,           
    avg_check NUMERIC
) AS $$
BEGIN
    IF start_date IS NULL OR end_date IS NULL THEN
        RAISE EXCEPTION 'start_date and end_date cannot be NULL';
    END IF;

    RETURN QUERY
    SELECT
        c."Id",
        c."Location",
        COUNT(a."Id")::BIGINT,
        COUNT(*) FILTER (WHERE a."Status" = 1)::BIGINT,
        COUNT(*) FILTER (WHERE a."Status" = 2)::BIGINT,
        COUNT(*) FILTER (WHERE a."Status" = 0)::BIGINT,
        COALESCE(SUM(ch."TotalPrice"), 0),
        COALESCE(AVG(ch."TotalPrice"), 0)
    FROM "Clinics" c
    LEFT JOIN "Appointments" a ON c."Id" = a."ClinicId"
    LEFT JOIN "Checks" ch ON a."Id" = ch."AppointmentId"
    WHERE a."Date" BETWEEN start_date AND end_date   
    GROUP BY c."Id", c."Location";
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_clinic_employee_count()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE "Clinics"
    SET "EmployeesCount" = (
        SELECT COUNT(*) 
        FROM "ClinicEmployees" 
        WHERE "ClinicId" = NEW."ClinicId"
    )
    WHERE "Id" = NEW."ClinicId";
    
    IF TG_OP = 'DELETE' THEN
        UPDATE "Clinics"
        SET "EmployeesCount" = (
            SELECT COUNT(*) 
            FROM "ClinicEmployees" 
            WHERE "ClinicId" = OLD."ClinicId"
        )
        WHERE "Id" = OLD."ClinicId";
        RETURN OLD;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_access_for_doctors()
RETURNS TRIGGER AS $$
	BEGIN
		IF NEW."IsCertified" = true AND (OLD."IsCertified" IS DISTINCT FROM true) THEN
			INSERT INTO "DoctorMaterialAccesses" ("EmployeeId", "MaterialId")
				SELECT NEW."Id", m."Id"
				FROM "Materials" m
				WHERE m."IsCertifiedMaterial" = true;
		END IF;
		RETURN NEW;
	END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION remove_client_bonuses()
RETURNS TRIGGER AS $$
	DECLARE
        discount INT;
	    client_id INT;
	    bonus_record RECORD;
	    remaining_discount INT;
	BEGIN
	    discount = NEW."Discount";
	    client_id = NEW."ClientId";
	    remaining_discount = discount;
	    
	    IF discount = 0 OR discount IS NULL THEN
	        RETURN NEW;
	    END IF;
	    
	    FOR bonus_record IN 
	        SELECT "Id", "Amount" 
	        FROM "Bonuses" 
	        WHERE "ClientId" = client_id 
	          AND "ExpiredAt" > NOW()
	          AND "Amount" > 0
	        ORDER BY "ExpiredAt" ASC  --сначала истекающие
	    LOOP
	        IF remaining_discount <= 0 THEN
	            EXIT;
	        END IF;
	        
	        IF bonus_record."Amount" <= remaining_discount THEN
	            remaining_discount = remaining_discount - bonus_record."Amount";
	            DELETE FROM "Bonuses" WHERE "Id" = bonus_record."Id";
	        ELSE
	            UPDATE "Bonuses" 
	            SET "Amount" = "Amount" - remaining_discount
	            WHERE "Id" = bonus_record."Id";
	            
	            remaining_discount = 0;
	        END IF;
	    END LOOP;	    
	    RETURN NEW;		
	END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION create_check()
RETURNS TRIGGER AS $$
	BEGIN
		IF NEW."Status" = 1 THEN
			INSERT INTO "Checks" ("Date", "TotalPrice", "Discount", "AppointmentId", "ClientId")
			VALUES (
				NOW(),
				NEW."TotalPrice",
				NEW."Discount",
				NEW."Id",
				NEW."ClientId"
			);

			UPDATE "Materials" m
			SET "Count" = m."Count" - sub.total
			FROM (
			    SELECT "MaterialId", SUM("Quantity") as total
			    FROM "AppointmentMaterial"
			    WHERE "AppointmentId" = NEW."Id"
			    GROUP BY "MaterialId"
			) sub
			WHERE m."Id" = sub."MaterialId";
		END IF;
		RETURN NEW;
	END 
$$ LANGUAGE plpgsql;


CREATE OR REPLACE TRIGGER trg_clinic_employee_insert
	AFTER INSERT OR DELETE ON "ClinicEmployees"
	FOR EACH ROW EXECUTE FUNCTION update_clinic_employee_count();

CREATE TRIGGER update_money_spent 
	AFTER INSERT OR UPDATE OR DELETE ON "Checks"
	FOR EACH ROW EXECUTE FUNCTION update_client_status();

CREATE OR REPLACE TRIGGER add_bonuses_for_check
	AFTER INSERT ON "Checks"
	FOR EACH ROW EXECUTE FUNCTION add_bonuses_for_client();

CREATE TRIGGER doctors_is_certified
	BEFORE UPDATE OF "IsCertified" ON "Employees"
	FOR EACH ROW EXECUTE FUNCTION add_access_for_doctors();

CREATE TRIGGER trg_add_access_new_material
    AFTER INSERT OR UPDATE ON "Materials"
    FOR EACH ROW
    EXECUTE FUNCTION add_access_new_material();

CREATE TRIGGER trg_remove_client_bonuses	
	AFTER INSERT ON "Appointments"
	FOR EACH ROW 
	EXECUTE FUNCTION remove_client_bonuses();

CREATE OR REPLACE TRIGGER trg_create_check
	AFTER UPDATE ON "Appointments"
	FOR EACH ROW 
	EXECUTE FUNCTION create_check();