using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterials_Appointments_AppointmentId",
                table: "AppointmentMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterials_Materials_MaterialId",
                table: "AppointmentMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployee_Clinics_ClinicsId",
                table: "ClinicEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployee_Employees_EmployeesId",
                table: "ClinicEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClinicEmployee",
                table: "ClinicEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentMaterials",
                table: "AppointmentMaterials");

            migrationBuilder.RenameTable(
                name: "AppointmentMaterials",
                newName: "AppointmentMaterial");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "ClinicEmployee",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "ClinicsId",
                table: "ClinicEmployee",
                newName: "ClinicId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployee_EmployeesId",
                table: "ClinicEmployee",
                newName: "IX_ClinicEmployee_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentMaterials_MaterialId",
                table: "AppointmentMaterial",
                newName: "IX_AppointmentMaterial_MaterialId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ClinicEmployee",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Checks",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Checks",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Bonuses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "AppointmentMaterial",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "AppointmentMaterial",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClinicEmployee",
                table: "ClinicEmployee",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentMaterial",
                table: "AppointmentMaterial",
                columns: new[] { "AppointmentId", "MaterialId" });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Name",
                table: "Materials",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_FirstName_SecondName",
                table: "Employees",
                columns: new[] { "FirstName", "SecondName" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Position",
                table: "Employees",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicEmployee_ClinicId",
                table: "ClinicEmployee",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicEmployee_EmployeeId_ClinicId",
                table: "ClinicEmployee",
                columns: new[] { "EmployeeId", "ClinicId" });

            migrationBuilder.CreateIndex(
                name: "IX_Checks_Date",
                table: "Checks",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_ExpiredAt",
                table: "Bonuses",
                column: "ExpiredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Date",
                table: "Appointments",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMaterial_AppointmentId",
                table: "AppointmentMaterial",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMaterial_Appointments_AppointmentId",
                table: "AppointmentMaterial",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMaterial_Materials_MaterialId",
                table: "AppointmentMaterial",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterial_Appointments_AppointmentId",
                table: "AppointmentMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterial_Materials_MaterialId",
                table: "AppointmentMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployee_Clinics_ClinicId",
                table: "ClinicEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_ClinicEmployee_Employees_EmployeeId",
                table: "ClinicEmployee");

            migrationBuilder.DropIndex(
                name: "IX_Materials_Name",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Employees_FirstName_SecondName",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Position",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClinicEmployee",
                table: "ClinicEmployee");

            migrationBuilder.DropIndex(
                name: "IX_ClinicEmployee_ClinicId",
                table: "ClinicEmployee");

            migrationBuilder.DropIndex(
                name: "IX_ClinicEmployee_EmployeeId_ClinicId",
                table: "ClinicEmployee");

            migrationBuilder.DropIndex(
                name: "IX_Checks_Date",
                table: "Checks");

            migrationBuilder.DropIndex(
                name: "IX_Bonuses_ExpiredAt",
                table: "Bonuses");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Date",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentMaterial",
                table: "AppointmentMaterial");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentMaterial_AppointmentId",
                table: "AppointmentMaterial");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ClinicEmployee");

            migrationBuilder.RenameTable(
                name: "AppointmentMaterial",
                newName: "AppointmentMaterials");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "ClinicEmployee",
                newName: "EmployeesId");

            migrationBuilder.RenameColumn(
                name: "ClinicId",
                table: "ClinicEmployee",
                newName: "ClinicsId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicEmployee_EmployeeId",
                table: "ClinicEmployee",
                newName: "IX_ClinicEmployee_EmployeesId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentMaterial_MaterialId",
                table: "AppointmentMaterials",
                newName: "IX_AppointmentMaterials_MaterialId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Checks",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Discount",
                table: "Checks",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Bonuses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "AppointmentMaterials",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClinicEmployee",
                table: "ClinicEmployee",
                columns: new[] { "ClinicsId", "EmployeesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentMaterials",
                table: "AppointmentMaterials",
                columns: new[] { "AppointmentId", "MaterialId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMaterials_Appointments_AppointmentId",
                table: "AppointmentMaterials",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentMaterials_Materials_MaterialId",
                table: "AppointmentMaterials",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployee_Clinics_ClinicsId",
                table: "ClinicEmployee",
                column: "ClinicsId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicEmployee_Employees_EmployeesId",
                table: "ClinicEmployee",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
