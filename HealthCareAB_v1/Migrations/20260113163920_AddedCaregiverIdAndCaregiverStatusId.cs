using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations
{
    /// <inheritdoc />
    public partial class AddedCaregiverIdAndCaregiverStatusId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaregiverDailySchedules_Caregivers_CaregiverUserId",
                table: "CaregiverDailySchedules");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "CaregiverDailySchedules",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "CaregiverDailySchedules",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "CaregiverUserId",
                table: "CaregiverDailySchedules",
                newName: "CaregiverId");

            migrationBuilder.RenameIndex(
                name: "IX_CaregiverDailySchedules_CaregiverUserId",
                table: "CaregiverDailySchedules",
                newName: "IX_CaregiverDailySchedules_CaregiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaregiverDailySchedules_Caregivers_CaregiverId",
                table: "CaregiverDailySchedules",
                column: "CaregiverId",
                principalTable: "Caregivers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaregiverDailySchedules_Caregivers_CaregiverId",
                table: "CaregiverDailySchedules");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "CaregiverDailySchedules",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "CaregiverDailySchedules",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "CaregiverId",
                table: "CaregiverDailySchedules",
                newName: "CaregiverUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CaregiverDailySchedules_CaregiverId",
                table: "CaregiverDailySchedules",
                newName: "IX_CaregiverDailySchedules_CaregiverUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaregiverDailySchedules_Caregivers_CaregiverUserId",
                table: "CaregiverDailySchedules",
                column: "CaregiverUserId",
                principalTable: "Caregivers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
