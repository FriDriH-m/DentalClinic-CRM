SELECT rolname FROM pg_roles;

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

--ROLES--
ALTER ROLE api_user CREATEROLE;
-- Username=api_user;Password=secure_password
GRANT role_admin TO api_user WITH ADMIN OPTION;
GRANT role_analyst TO api_user WITH ADMIN OPTION;
GRANT role_doctor TO api_user WITH ADMIN OPTION;
GRANT role_manager TO api_user WITH ADMIN OPTION;

GRANT "role_doctor" TO "role_admin" WITH ADMIN OPTION;
GRANT "role_analyst" TO "role_admin" WITH ADMIN OPTION;
GRANT "role_manager" TO "role_admin" WITH ADMIN OPTION;

CREATE ROLE role_manager;
CREATE ROLE role_admin;
CREATE ROLE role_doctor;
CREATE ROLE role_analyst;
ALTER ROLE role_admin BYPASSRLS;
---------------

--GRANTS--

GRANT SELECT ON ALL TABLES IN SCHEMA public TO role_analyst;

GRANT SELECT ON TABLE "Appointments" TO role_doctor;
GRANT SELECT, INSERT ON TABLE "AppointmentMaterials", "Materials" TO role_doctor;
GRANT INSERT ON TABLE "Checks" TO role_doctor;

ALTER ROLE role_admin CREATEROLE;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO role_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO role_admin;

GRANT ALL ON ALL TABLES IN SCHEMA public TO role_admin;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO role_admin;

GRANT SELECT, INSERT, UPDATE ON TABLE "Clients", "Appointments", "Checks" TO role_manager;
GRANT DELETE, UPDATE ON TABLE "Bonuses" TO role_manager;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO role_manager;
-------------
ALTER TABLE "Clients" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Appointments" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Checks" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Bonuses" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Materials" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Employees" ENABLE ROW LEVEL SECURITY;

CREATE POLICY api_employee_access ON "Employees"
    FOR ALL
    TO api_user
    USING (true) 
    WITH CHECK (true);

CREATE POLICY analyst_employee_access ON "Employees"
    FOR ALL
    TO role_analyst
    USING (true) 
    WITH CHECK (true);


CREATE POLICY manager_clients_access ON "Clients"
    FOR ALL
    TO role_manager
    USING (true) 
    WITH CHECK (true);

CREATE POLICY manager_clients_access ON "Clients"
    FOR ALL
    TO role_manager
    USING (true) 
    WITH CHECK (true);
	
CREATE POLICY manager_employees_access ON "Employees"
    FOR ALL 
    TO role_manager
    USING (
        "Id" IN (
            SELECT ce."EmployeeId"
            FROM "ClinicEmployees" ce
            WHERE ce."ClinicId" IN (
                SELECT ce2."ClinicId"
                FROM "ClinicEmployees" ce2
                WHERE ce2."EmployeeId" = current_setting('app.current_employee_id')::int
            )
        )
    )
    WITH CHECK (
        "Id" IN (
            SELECT ce."EmployeeId"
            FROM "ClinicEmployees" ce
            WHERE ce."ClinicId" IN (
                SELECT ce2."ClinicId"
                FROM "ClinicEmployees" ce2
                WHERE ce2."EmployeeId" = current_setting('app.current_employee_id')::int
            )
        )
    );

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
CREATE POLICY manager_employees_access ON "Employees"
	FOR ALL 
	TO role_manager
	USING(
		""
	)

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