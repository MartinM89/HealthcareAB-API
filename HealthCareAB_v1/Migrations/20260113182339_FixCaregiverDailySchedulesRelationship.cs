using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations;

[ExcludeFromCodeCoverage]
/// <inheritdoc />
public partial class FixCaregiverDailySchedulesRelationship : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Bookings_Patients_PatientUserId",
            table: "Bookings"
        );

        migrationBuilder.RenameColumn(
            name: "PatientUserId",
            table: "Bookings",
            newName: "PatientId"
        );

        migrationBuilder.RenameIndex(
            name: "IX_Bookings_PatientUserId",
            table: "Bookings",
            newName: "IX_Bookings_PatientId"
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Patients_PatientId",
            table: "Bookings",
            column: "PatientId",
            principalTable: "Patients",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Cascade
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Bookings_Patients_PatientId", table: "Bookings");

        migrationBuilder.RenameColumn(
            name: "PatientId",
            table: "Bookings",
            newName: "PatientUserId"
        );

        migrationBuilder.RenameIndex(
            name: "IX_Bookings_PatientId",
            table: "Bookings",
            newName: "IX_Bookings_PatientUserId"
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Patients_PatientUserId",
            table: "Bookings",
            column: "PatientUserId",
            principalTable: "Patients",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Cascade
        );
    }
}
