using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBookingRelationFromUserToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Patients_PatientUserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientUserId",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Patients_PatientUserId",
                table: "Bookings",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Patients_PatientUserId",
                table: "Bookings");

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientUserId",
                table: "Bookings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId1",
                table: "Bookings",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Patients_PatientUserId",
                table: "Bookings",
                column: "PatientUserId",
                principalTable: "Patients",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId1",
                table: "Bookings",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
