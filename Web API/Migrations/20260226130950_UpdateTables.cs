using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentService_AppointmentMaterials_ServicesId",
                table: "AppointmentService");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AppointmentMaterials_ServiceId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "AppointmentMaterial");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Login",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_RoleId",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentMaterials",
                table: "AppointmentMaterials");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AppointmentMaterials");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AppointmentMaterials");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "Employees",
                newName: "DbUsername");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Materials",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Materials",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Appointments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Appointments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "AppointmentMaterials",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentMaterials",
                table: "AppointmentMaterials",
                columns: new[] { "AppointmentId", "MaterialId" });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMaterials_MaterialId",
                table: "AppointmentMaterials",
                column: "MaterialId");

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
                name: "FK_AppointmentService_Services_ServicesId",
                table: "AppointmentService",
                column: "ServicesId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Services_ServiceId",
                table: "Employees",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterials_Appointments_AppointmentId",
                table: "AppointmentMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentMaterials_Materials_MaterialId",
                table: "AppointmentMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentService_Services_ServicesId",
                table: "AppointmentService");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Services_ServiceId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentMaterials",
                table: "AppointmentMaterials");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentMaterials_MaterialId",
                table: "AppointmentMaterials");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "AppointmentMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "AppointmentMaterials");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "AppointmentMaterials");

            migrationBuilder.RenameColumn(
                name: "DbUsername",
                table: "Employees",
                newName: "Login");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Materials",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Employees",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TotalPrice",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AppointmentMaterials",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AppointmentMaterials",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AppointmentMaterials",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentMaterials",
                table: "AppointmentMaterials",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppointmentMaterial",
                columns: table => new
                {
                    AppointmentsId = table.Column<int>(type: "integer", nullable: false),
                    MaterialsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentMaterial", x => new { x.AppointmentsId, x.MaterialsId });
                    table.ForeignKey(
                        name: "FK_AppointmentMaterial_Appointments_AppointmentsId",
                        column: x => x.AppointmentsId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentMaterial_Materials_MaterialsId",
                        column: x => x.MaterialsId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Analyst" },
                    { 3, "Doctor" },
                    { 4, "Manager" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Login",
                table: "Employees",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleId",
                table: "Employees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentMaterial_MaterialsId",
                table: "AppointmentMaterial",
                column: "MaterialsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentService_AppointmentMaterials_ServicesId",
                table: "AppointmentService",
                column: "ServicesId",
                principalTable: "AppointmentMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AppointmentMaterials_ServiceId",
                table: "Employees",
                column: "ServiceId",
                principalTable: "AppointmentMaterials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
