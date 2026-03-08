using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Services_ServiceId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ServiceId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Employees",
                newName: "Specialization");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_Position",
                table: "Employees",
                newName: "IX_Employees_Specialization");

            migrationBuilder.CreateTable(
                name: "DoctorCategorySkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorCategorySkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorCategorySkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorCategorySkills_EmployeeId",
                table: "DoctorCategorySkills",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorCategorySkills");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "Employees",
                newName: "Position");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_Specialization",
                table: "Employees",
                newName: "IX_Employees_Position");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Employees",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ServiceId",
                table: "Employees",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Services_ServiceId",
                table: "Employees",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
