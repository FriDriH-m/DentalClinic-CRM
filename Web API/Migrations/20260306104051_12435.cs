using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class _12435 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployee_Clinics_ClinicId",
                table: "ClinicEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployee_Employees_EmployeeId",
                table: "ClinicEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClinicEmployee",
                table: "ClinicEmployee");

            migrationBuilder.RenameTable(
                name: "ClinicEmployee",
                newName: "ClinicEmployees");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployee_EmployeeId_ClinicId",
                table: "ClinicEmployees",
                newName: "IX_ClinicEmployees_EmployeeId_ClinicId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployee_EmployeeId",
                table: "ClinicEmployees",
                newName: "IX_ClinicEmployees_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployee_ClinicId",
                table: "ClinicEmployees",
                newName: "IX_ClinicEmployees_ClinicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClinicEmployees",
                table: "ClinicEmployees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployees_Clinics_ClinicId",
                table: "ClinicEmployees",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployees_Employees_EmployeeId",
                table: "ClinicEmployees",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployees_Clinics_ClinicId",
                table: "ClinicEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployees_Employees_EmployeeId",
                table: "ClinicEmployees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClinicEmployees",
                table: "ClinicEmployees");

            migrationBuilder.RenameTable(
                name: "ClinicEmployees",
                newName: "ClinicEmployee");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployees_EmployeeId_ClinicId",
                table: "ClinicEmployee",
                newName: "IX_ClinicEmployee_EmployeeId_ClinicId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployees_EmployeeId",
                table: "ClinicEmployee",
                newName: "IX_ClinicEmployee_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployees_ClinicId",
                table: "ClinicEmployee",
                newName: "IX_ClinicEmployee_ClinicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClinicEmployee",
                table: "ClinicEmployee",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployee_Clinics_ClinicId",
                table: "ClinicEmployee",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployee_Employees_EmployeeId",
                table: "ClinicEmployee",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
