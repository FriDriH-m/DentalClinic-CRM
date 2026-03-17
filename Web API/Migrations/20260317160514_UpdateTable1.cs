using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorCategorySkills_Materials_MaterialId",
                table: "DoctorCategorySkills");

            migrationBuilder.DropIndex(
                name: "IX_DoctorCategorySkills_MaterialId",
                table: "DoctorCategorySkills");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "DoctorCategorySkills");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "ServiceMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "ServiceMaterials");

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "DoctorCategorySkills",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorCategorySkills_MaterialId",
                table: "DoctorCategorySkills",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorCategorySkills_Materials_MaterialId",
                table: "DoctorCategorySkills",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }
    }
}
