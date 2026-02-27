using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddChecksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ClinicId",
                table: "Services",
                column: "ClinicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Clinics_ClinicId",
                table: "Services",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Clinics_ClinicId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ClinicId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Services");
        }
    }
}
