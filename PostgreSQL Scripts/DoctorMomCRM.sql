UPDATE "Employees"
SET "IsCertified" = NULL
WHERE "Id" = 13;

INSERT INTO "Bonuses" ("AddedAt", "ExpiredAt", "Amount", "ClientId")
SELECT 
    NOW() - (interval '1 day' * g.s),                    -- Добавлены в разные дни
    NOW() + (interval '30 day' * (random() * 5 + 1)),    -- Истекают через 30-180 дней
    (random() * 100 + 10)::int,                          -- Сумма от 10 до 110
    7                                                     -- ClientId
FROM generate_series(1, 500) AS g(s);
SELECT * FROM "AppointmentService";
SELECT * FROM "Appointments";
SELECT * FROM "AppointmentMaterial";
SELECT * FROM "Clients";
SELECT * FROM "Clinics";
SELECT * FROM "Checks";
SELECT * FROM "Materials";
SELECT * FROM "Services";
SELECT * FROM "ServiceMaterials";
SELECT * FROM "Employees";	
SELECT * FROM "ClinicEmployees";
SELECT * FROM "DoctorCategorySkills";
SELECT * FROM "DoctorMaterialAccesses";
SELECT * FROM "Bonuses";

DELETE FROM "Appointments" AS a WHERE a."Id" IN (18, 17, 22, 23, 24, 25);

SELECT grantee, privilege_type 
FROM information_schema.role_table_grants 
WHERE table_name = 'Clinics';

SELECT rolname, rolcanlogin, rolsuper, rolcreaterole 
FROM pg_roles 
WHERE rolname NOT LIKE 'pg_%';

SELECT r.rolname
            FROM pg_roles r
            JOIN pg_auth_members m ON r.oid = m.roleid
            JOIN pg_roles u ON u.oid = m.member
			WHERE u.rolname = 'manager_fedor';

SELECT * FROM pg_roles;
SELECT * FROM pg_auth_members;