ALTER ROLE api_user CREATEROLE;
-- Username=api_user;Password=secure_password
GRANT role_admin TO api_user WITH ADMIN OPTION;
GRANT role_analyst TO api_user WITH ADMIN OPTION;
GRANT role_doctor TO api_user WITH ADMIN OPTION;
GRANT role_manager TO api_user WITH ADMIN OPTION;

GRANT "role_doctor" TO "role_admin" WITH ADMIN OPTION;
GRANT "role_analyst" TO "role_admin" WITH ADMIN OPTION;
GRANT "role_manager" TO "role_admin" WITH ADMIN OPTION;

DROP POLICY IF EXISTS manager_clients_access ON "Clients";
DROP POLICY IF EXISTS manager_appointments_isolation ON "Appointments";
DROP POLICY IF EXISTS manager_checks_access ON "Checks";
DROP POLICY IF EXISTS manager_bonuses_access ON "Bonuses";
DROP POLICY IF EXISTS manager_materials_access ON "Materials";

DROP POLICY IF EXISTS analyst_full_access ON "Clients";
DROP POLICY IF EXISTS analyst_appointments_access ON "Appointments";
DROP POLICY IF EXISTS analyst_materials_access ON "Materials";
DROP POLICY IF EXISTS analyst_checks_access ON "Checks";
DROP POLICY IF EXISTS analyst_bonuses_access ON "Bonuses";

DROP POLICY IF EXISTS doctor_client_isolation ON "Clients";
DROP POLICY IF EXISTS doctor_materials_isolation ON "Materials";
DROP POLICY IF EXISTS doctor_appointments_isolation ON "Appointments";

SELECT * FROM "Clinics";
SELECT * FROM "Services";
SELECT * FROM "Employees";
SELECT * FROM "ClinicEmployees";
SELECT * FROM "DoctorCategorySkills";

SELECT grantee, privilege_type 
FROM information_schema.role_table_grants 
WHERE table_name = 'DoctorCategorySkills';

SELECT rolname, rolcanlogin, rolsuper, rolcreaterole 
FROM pg_roles 
WHERE rolname NOT LIKE 'pg_%';

SELECT r.rolname
            FROM pg_roles r
            JOIN pg_auth_members m ON r.oid = m.roleid
            JOIN pg_roles u ON u.oid = m.member
			WHERE u.rolname = 'JEI';

SELECT * FROM pg_roles;
SELECT * FROM pg_auth_members;

CREATE ROLE role_manager;
CREATE ROLE role_admin;
CREATE ROLE role_doctor;
CREATE ROLE role_analyst;
ALTER ROLE role_admin BYPASSRLS;

GRANT SELECT ON ALL TABLES IN SCHEMA public TO role_analyst;

GRANT SELECT ON TABLE "Appointments" TO role_doctor;
GRANT SELECT, INSERT ON TABLE "AppointmentMaterials", "Materials" TO role_doctor;
GRANT INSERT ON TABLE "Checks" TO role_doctor;

ALTER ROLE role_admin CREATEROLE;
GRANT ALL ON ALL TABLES IN SCHEMA public TO role_admin;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO role_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO role_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO role_admin;

SELECT relname, relrowsecurity FROM pg_class WHERE relname = 'DoctorCategorySkills';
SELECT grantee, privilege_type 
FROM information_schema.role_table_grants 
WHERE table_name = 'DoctorCategorySkills';


GRANT SELECT, INSERT, UPDATE ON TABLE "Clients", "Appointments", "Checks" TO role_manager;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO role_manager;

ALTER TABLE "Clients" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Appointments" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Checks" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Bonuses" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Materials" ENABLE ROW LEVEL SECURITY;

CREATE POLICY manager_clients_access ON "Clients"
    FOR ALL
    TO role_manager
    USING (true) 
    WITH CHECK (true);

CREATE POLICY manager_appointments_isolation ON "Appointments"
    FOR ALL
    TO role_manager
    USING (
        "ClinicId" IN (
            SELECT ce."ClinicId"
            FROM "ClinicEmployee" ce
            WHERE ce."EmployeeId" = current_setting('app.current_employee_id')::int
        )
    )
    WITH CHECK (
        "ClinicId" IN (
            SELECT ce."ClinicId"
            FROM "ClinicEmployee" ce
            WHERE ce."EmployeeId" = current_setting('app.current_employee_id')::int
        )
    );

CREATE POLICY manager_checks_access ON "Checks"
    FOR ALL
    TO role_manager
    USING (
        "AppointmentId" IN (
            SELECT a."Id"
            FROM "Appointments" a
            WHERE a."ClinicId" IN (
                SELECT ce."ClinicId"
                FROM "ClinicEmployee" ce
                WHERE ce."EmployeeId" = current_setting('app.current_employee_id')::int
            )
        )
    )
    WITH CHECK (
        "AppointmentId" IN (
            SELECT a."Id"
            FROM "Appointments" a
            WHERE a."ClinicId" IN (
                SELECT ce."ClinicId"
                FROM "ClinicEmployee" ce
                WHERE ce."EmployeeId" = current_setting('app.current_employee_id')::int
            )
        )
    );

CREATE POLICY manager_bonuses_access ON "Bonuses"
    FOR SELECT
    TO role_manager
    USING (true); 

CREATE POLICY manager_materials_access ON "Materials"
    FOR SELECT
    TO role_manager
    USING (
        "ClinicId" IN (
            SELECT ce."ClinicId"
            FROM "ClinicEmployee" ce
            WHERE ce."EmployeeId" = current_setting('app.current_employee_id')::int
        )
    );

CREATE POLICY analyst_full_access ON "Clients"
    FOR SELECT TO role_analyst USING (true);

CREATE POLICY analyst_appointments_access ON "Appointments"
    FOR SELECT TO role_analyst USING (true);

CREATE POLICY analyst_materials_access ON "Materials"
    FOR SELECT TO role_analyst USING (true);

CREATE POLICY analyst_checks_access ON "Checks"
    FOR SELECT TO role_analyst USING (true);

CREATE POLICY analyst_bonuses_access ON "Bonuses"
    FOR SELECT TO role_analyst USING (true);

CREATE POLICY doctor_client_isolation ON "Clients"
    FOR SELECT
    TO role_doctor
    USING (
        "Id" IN (
            SELECT apnt."ClientId" 
            FROM "Appointments" apnt
            WHERE apnt."EmployeeId" = current_setting('app.current_employee_id')::int
        )
    );

CREATE POLICY doctor_materials_isolation ON "Materials"
    FOR ALL
    TO role_doctor
    USING(
        "ClinicId" in (
            SELECT clinic."ClinicId"
            FROM "ClinicEmployee" clinic
            WHERE clinic."EmployeeId" = current_setting('app.current_employee_id')::int
        )
    )
    WITH CHECK(
        "ClinicId" IN (
            SELECT clinic."ClinicId"
            FROM "ClinicEmployee" clinic
            WHERE clinic."EmployeeId" = current_setting('app.current_employee_id')::int
        )
    );

CREATE POLICY doctor_appointments_isolation ON "Appointments"
    FOR SELECT 
    TO role_doctor
    USING("EmployeeId" = current_setting('app.current_employee_id')::int);


-- Исправленная функция update_client_status (теперь корректно обрабатывает DELETE)
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
		
		RETURN NULL;  -- для AFTER-триггера возвращаемое значение игнорируется, но принято возвращать NULL
	END;
$$ LANGUAGE plpgsql;

CREATE FUNCTION add_bonuses_for_client()
	RETURNS TRIGGER AS $$
	BEGIN		
		INSERT INTO "Bonuses" ("AddedAt", "ExpiredAt", "Amount", "ClientId")
		VALUES (
			NOW(), 
			NOW() + INTERVAL '1 year',
			FLOOR(NEW."TotalPrice" * 0.10)::INT, 
			NEW."ClientId"
        );
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
    SET "EmployeesCount" = "EmployeesCount" + 1
    WHERE "Id" = NEW."ClinicId";
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_clinic_employee_insert
AFTER INSERT ON "ClinicEmployees"
FOR EACH ROW
EXECUTE FUNCTION update_clinic_employee_count();

CREATE TRIGGER update_money_spent 
	AFTER INSERT OR UPDATE OR DELETE ON "Checks"
	FOR EACH ROW EXECUTE FUNCTION update_client_status();

CREATE TRIGGER add_bonuses_for_check
	AFTER INSERT ON "Checks"
	FOR EACH ROW EXECUTE FUNCTION add_bonuses_for_client();
INSERT INTO "Clinics" ("Id", "Location", "PostalCode", "PhoneNumber", "EmployeesCount")
VALUES 
    (1, 'Москва, ул. Ленина 10', '101000', '+7(495)111-11-11', 0),
    (2, 'Москва, ул. Мира 25', '101001', '+7(495)222-22-22', 0),
    (3, 'Санкт-Петербург, Невский пр. 50', '190000', '+7(812)333-33-33', 0);

INSERT INTO "Services" ("Name", "Description", "DurationMinutes", "CategoryId", "CategoryName", "ClinicId", "BasePrice")
VALUES 
    -- Категория 1: Диагностика
    ('Первичная консультация', 'Осмотр, консультация врача, составление плана лечения', 30, 1, 'Диагностика', 1, 1000),
    ('Повторная консультация', 'Консультация врача после начала лечения', 20, 1, 'Диагностика', 1, 500),
    ('Рентген зуба', 'Прицельный рентгеновский снимок', 10, 1, 'Диагностика', 1, 600),
    ('Панорамный снимок (ОПТГ)', 'Ортопантомограмма всех зубов', 15, 1, 'Диагностика', 1, 1500),
    ('КТ челюсти', 'Компьютерная томография', 20, 1, 'Диагностика', 1, 3000),
    
    -- Категория 2: Терапия (Лечение кариеса)
    ('Лечение кариеса (поверхностный)', 'Лечение кариеса без осложнения', 45, 2, 'Терапия', 1, 3500),
    ('Лечение кариеса (средний)', 'Лечение кариеса средней степени', 60, 2, 'Терапия', 1, 4500),
    ('Лечение кариеса (глубокий)', 'Лечение глубокого кариеса', 75, 2, 'Терапия', 1, 5500),
    ('Лечение пульпита (1 канал)', 'Лечение воспаления нерва, 1 канал', 90, 2, 'Терапия', 1, 6000),
    ('Лечение пульпита (2 канала)', 'Лечение воспаления нерва, 2 канала', 120, 2, 'Терапия', 1, 8000),
    ('Лечение пульпита (3 канала)', 'Лечение воспаления нерва, 3 канала', 150, 2, 'Терапия', 1, 10000),
    ('Лечение периодонтита', 'Лечение воспаления вокруг корня', 90, 2, 'Терапия', 1, 7000),
    ('Художественная реставрация', 'Восстановление зуба композитом', 90, 2, 'Терапия', 1, 6500),
    
    -- Категория 3: Гигиена и профилактика
    ('Профессиональная чистка', 'Ультразвуковая чистка + Air Flow', 60, 3, 'Гигиена', 1, 4500),
    ('Ультразвуковая чистка', 'Снятие зубных отложений ультразвуком', 45, 3, 'Гигиена', 1, 3000),
    ('Air Flow', 'Пескоструйная чистка', 30, 3, 'Гигиена', 1, 2500),
    ('Покрытие фторлаком', 'Укрепление эмали фтором (1 зуб)', 10, 3, 'Гигиена', 1, 300),
    ('Герметизация фиссур', 'Запечатывание бороздок зуба', 30, 3, 'Гигиена', 1, 1500),
    
    -- Категория 4: Хирургия
    ('Удаление зуба (простое)', 'Удаление однокорневого зуба', 30, 4, 'Хирургия', 1, 2500),
    ('Удаление зуба (сложное)', 'Удаление многокорневого зуба', 60, 4, 'Хирургия', 1, 4500),
    ('Удаление зуба мудрости (простое)', 'Удаление ретинированного зуба', 45, 4, 'Хирургия', 1, 5000),
    ('Удаление зуба мудрости (сложное)', 'Удаление сложного зуба мудрости', 90, 4, 'Хирургия', 1, 8000),
    ('Вскрытие абсцесса', 'Вскрытие гнойника', 30, 4, 'Хирургия', 1, 2000),
    ('Пластика уздечки', 'Коррекция уздечки языка/губы', 30, 4, 'Хирургия', 1, 3500),
    
    -- Категория 5: Имплантация
    ('Имплантация (установка импланта)', 'Установка зубного импланта', 90, 5, 'Имплантация', 1, 35000),
    ('Имплантация премиум', 'Установка импланта премиум-класса', 90, 5, 'Имплантация', 1, 55000),
    ('Синус-лифтинг (открытый)', 'Наращивание костной ткани', 120, 5, 'Имплантация', 1, 25000),
    ('Формирователь десны', 'Установка формирователя', 30, 5, 'Имплантация', 1, 3000),
    
    -- Категория 6: Протезирование
    ('Металлокерамическая коронка', 'Коронка из металлокерамики', 120, 6, 'Протезирование', 1, 12000),
    ('Циркониевая коронка', 'Коронка из диоксида циркония', 120, 6, 'Протезирование', 1, 25000),
    ('Керамическая коронка E-max', 'Безметалловая коронка', 120, 6, 'Протезирование', 1, 22000),
    ('Винир керамический', 'Установка винира', 150, 6, 'Протезирование', 1, 30000),
    ('Съемный протез', 'Изготовление съемного протеза', 180, 6, 'Протезирование', 1, 25000),
    ('Мостовидный протез', 'Установка моста (3 единицы)', 180, 6, 'Протезирование', 1, 35000),
    
    -- Категория 7: Ортодонтия
    ('Брекеты металлические', 'Установка металлических брекетов', 180, 7, 'Ортодонтия', 1, 45000),
    ('Брекеты керамические', 'Установка керамических брекетов', 180, 7, 'Ортодонтия', 1, 65000),
    ('Брекеты сапфировые', 'Установка сапфировых брекетов', 180, 7, 'Ортодонтия', 1, 75000),
    ('Элайнеры (Invisalign)', 'Лечение элайнерами', 60, 7, 'Ортодонтия', 1, 150000),
    ('Коррекция брекетов', 'Плановая коррекция', 45, 7, 'Ортодонтия', 1, 2500),
    
    -- Категория 8: Отбеливание
    ('Отбеливание Zoom 4', 'Профессиональное отбеливание', 120, 8, 'Отбеливание', 1, 25000),
    ('Лазерное отбеливание', 'Отбеливание лазером', 90, 8, 'Отбеливание', 1, 20000),
    ('Внутриканальное отбеливание', 'Отбеливание депульпированного зуба', 60, 8, 'Отбеливание', 1, 3500);

-- Для второй клиники (ClinicId = 2)
INSERT INTO "Services" ("Name", "Description", "DurationMinutes", "CategoryId", "CategoryName", "ClinicId", "BasePrice")
VALUES 
    ('Первичная консультация', 'Осмотр, консультация врача', 30, 1, 'Диагностика', 2, 1000),
    ('Лечение кариеса', 'Лечение кариеса', 60, 2, 'Терапия', 2, 4000),
    ('Профессиональная чистка', 'Чистка зубов', 60, 3, 'Гигиена', 2, 4500),
    ('Удаление зуба', 'Удаление зуба', 30, 4, 'Хирургия', 2, 2500),
    ('Металлокерамическая коронка', 'Коронка', 120, 6, 'Протезирование', 2, 12000);

-- Для третьей клиники (ClinicId = 3)
INSERT INTO "Services" ("Name", "Description", "DurationMinutes", "CategoryId", "CategoryName", "ClinicId", "BasePrice")
VALUES 
    ('Первичная консультация', 'Осмотр, консультация врача', 30, 1, 'Диагностика', 3, 1200),
    ('Лечение кариеса', 'Лечение кариеса', 60, 2, 'Терапия', 3, 4500),
    ('Имплантация', 'Установка импланта', 90, 5, 'Имплантация', 3, 38000),
    ('Брекеты', 'Установка брекетов', 180, 7, 'Ортодонтия', 3, 50000);

