UPDATE "Employees"
SET "IsCertified" = NULL
WHERE "Id" = 13;



SELECT * FROM "Clinics";
SELECT * FROM "Materials";
SELECT * FROM "Services";
SELECT * FROM "ServiceMaterials";
SELECT * FROM "Employees";
SELECT * FROM "ClinicEmployees";
SELECT * FROM "DoctorCategorySkills";
SELECT * FROM "DoctorMaterialAccesses";

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