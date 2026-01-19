using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationForDailyschedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_CaregiverDailySchedules_CaregiverDailyScheduleId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CaregiverDailyScheduleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CaregiverDailyScheduleId",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "DailyScheduleId",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DailyScheduleId",
                table: "Bookings",
                column: "DailyScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_CaregiverDailySchedules_DailyScheduleId",
                table: "Bookings",
                column: "DailyScheduleId",
                principalTable: "CaregiverDailySchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_CaregiverDailySchedules_DailyScheduleId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DailyScheduleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DailyScheduleId",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "CaregiverDailyScheduleId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CaregiverDailyScheduleId",
                table: "Bookings",
                column: "CaregiverDailyScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_CaregiverDailySchedules_CaregiverDailyScheduleId",
                table: "Bookings",
                column: "CaregiverDailyScheduleId",
                principalTable: "CaregiverDailySchedules",
                principalColumn: "Id");
        }
    }
}
