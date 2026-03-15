using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableAppoinmentService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentService_Appointments_AppointmentsId",
                table: "AppointmentService");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentService_Services_ServicesId",
                table: "AppointmentService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService");

            migrationBuilder.RenameColumn(
                name: "ServicesId",
                table: "AppointmentService",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "AppointmentsId",
                table: "AppointmentService",
                newName: "AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentService_ServicesId",
                table: "AppointmentService",
                newName: "IX_AppointmentService_ServiceId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCertified",
                table: "Employees",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "AppointmentService",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_AppointmentId",
                table: "AppointmentService",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentService_Appointments_AppointmentId",
                table: "AppointmentService",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentService_Services_ServiceId",
                table: "AppointmentService",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentService_Appointments_AppointmentId",
                table: "AppointmentService");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentService_Services_ServiceId",
                table: "AppointmentService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentService_AppointmentId",
                table: "AppointmentService");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "AppointmentService");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "AppointmentService",
                newName: "ServicesId");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "AppointmentService",
                newName: "AppointmentsId");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentService_ServiceId",
                table: "AppointmentService",
                newName: "IX_AppointmentService_ServicesId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCertified",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentService",
                table: "AppointmentService",
                columns: new[] { "AppointmentsId", "ServicesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentService_Appointments_AppointmentsId",
                table: "AppointmentService",
                column: "AppointmentsId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentService_Services_ServicesId",
                table: "AppointmentService",
                column: "ServicesId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
