using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterial_Materials_MaterialId",
                table: "AppointmentMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployees_Clinics_ClinicId",
                table: "ClinicEmployees");

            migrationBuilder.DropColumn(
                name: "HasSpecialLicanse",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "DoctorCategorySkills",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DoctorMaterialAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorMaterialAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorMaterialAccesses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorMaterialAccesses_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceMaterials_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorCategorySkills_MaterialId",
                table: "DoctorCategorySkills",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMaterialAccesses_EmployeeId",
                table: "DoctorMaterialAccesses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMaterialAccesses_MaterialId",
                table: "DoctorMaterialAccesses",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMaterials_MaterialId",
                table: "ServiceMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceMaterials_ServiceId",
                table: "ServiceMaterials",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMaterial_Materials_MaterialId",
                table: "AppointmentMaterial",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployees_Clinics_ClinicId",
                table: "ClinicEmployees",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorCategorySkills_Materials_MaterialId",
                table: "DoctorCategorySkills",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterial_Materials_MaterialId",
                table: "AppointmentMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployees_Clinics_ClinicId",
                table: "ClinicEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorCategorySkills_Materials_MaterialId",
                table: "DoctorCategorySkills");

            migrationBuilder.DropTable(
                name: "DoctorMaterialAccesses");

            migrationBuilder.DropTable(
                name: "ServiceMaterials");

            migrationBuilder.DropIndex(
                name: "IX_DoctorCategorySkills_MaterialId",
                table: "DoctorCategorySkills");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "DoctorCategorySkills");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Appointments");

            migrationBuilder.AddColumn<bool>(
                name: "HasSpecialLicanse",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMaterial_Materials_MaterialId",
                table: "AppointmentMaterial",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployees_Clinics_ClinicId",
                table: "ClinicEmployees",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
