using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations;

[ExcludeFromCodeCoverage]
/// <inheritdoc />
public partial class AlteredUserPatientCaregiverRelationships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Bookings_Patients_PatientId", table: "Bookings");

        migrationBuilder.DropForeignKey(
            name: "FK_CaregiverDailySchedules_Caregivers_CaregiverId",
            table: "CaregiverDailySchedules"
        );

        migrationBuilder.DropColumn(name: "Email", table: "Patients");

        migrationBuilder.DropColumn(name: "FirstName", table: "Patients");

        migrationBuilder.DropColumn(name: "LastName", table: "Patients");

        migrationBuilder.DropColumn(name: "PasswordHash", table: "Patients");

        migrationBuilder.DropColumn(name: "PhoneNumber", table: "Patients");

        migrationBuilder.DropColumn(name: "Roles", table: "Patients");

        migrationBuilder.DropColumn(name: "Username", table: "Patients");

        migrationBuilder.DropColumn(name: "Email", table: "Caregivers");

        migrationBuilder.DropColumn(name: "FirstName", table: "Caregivers");

        migrationBuilder.DropColumn(name: "LastName", table: "Caregivers");

        migrationBuilder.DropColumn(name: "PasswordHash", table: "Caregivers");

        migrationBuilder.DropColumn(name: "PhoneNumber", table: "Caregivers");

        migrationBuilder.DropColumn(name: "Roles", table: "Caregivers");

        migrationBuilder.DropColumn(name: "Username", table: "Caregivers");

        migrationBuilder.RenameColumn(name: "Id", table: "Patients", newName: "UserId");

        migrationBuilder.RenameColumn(name: "Id", table: "Caregivers", newName: "UserId");

        migrationBuilder.RenameColumn(
            name: "CaregiverId",
            table: "CaregiverDailySchedules",
            newName: "CaregiverUserId"
        );

        migrationBuilder.RenameIndex(
            name: "IX_CaregiverDailySchedules_CaregiverId",
            table: "CaregiverDailySchedules",
            newName: "IX_CaregiverDailySchedules_CaregiverUserId"
        );

        migrationBuilder.RenameColumn(name: "PatientId", table: "Bookings", newName: "UserId1");

        migrationBuilder.RenameIndex(
            name: "IX_Bookings_PatientId",
            table: "Bookings",
            newName: "IX_Bookings_UserId1"
        );

        migrationBuilder.AddColumn<Guid>(
            name: "PatientUserId",
            table: "Bookings",
            type: "uuid",
            nullable: true
        );

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Username = table.Column<string>(
                    type: "character varying(50)",
                    maxLength: 50,
                    nullable: false
                ),
                PasswordHash = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                FirstName = table.Column<string>(type: "text", nullable: false),
                LastName = table.Column<string>(type: "text", nullable: false),
                PhoneNumber = table.Column<string>(type: "text", nullable: false),
                Roles = table.Column<string>(type: "jsonb", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_PatientUserId",
            table: "Bookings",
            column: "PatientUserId"
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Patients_PatientUserId",
            table: "Bookings",
            column: "PatientUserId",
            principalTable: "Patients",
            principalColumn: "UserId"
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Users_UserId1",
            table: "Bookings",
            column: "UserId1",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade
        );

        migrationBuilder.AddForeignKey(
            name: "FK_CaregiverDailySchedules_Caregivers_CaregiverUserId",
            table: "CaregiverDailySchedules",
            column: "CaregiverUserId",
            principalTable: "Caregivers",
            principalColumn: "UserId",
            onDelete: ReferentialAction.Cascade
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Caregivers_Users_UserId",
            table: "Caregivers",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Patients_Users_UserId",
            table: "Patients",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Bookings_Patients_PatientUserId",
            table: "Bookings"
        );

        migrationBuilder.DropForeignKey(name: "FK_Bookings_Users_UserId1", table: "Bookings");

        migrationBuilder.DropForeignKey(
            name: "FK_CaregiverDailySchedules_Caregivers_CaregiverUserId",
            table: "CaregiverDailySchedules"
        );

        migrationBuilder.DropForeignKey(name: "FK_Caregivers_Users_UserId", table: "Caregivers");

        migrationBuilder.DropForeignKey(name: "FK_Patients_Users_UserId", table: "Patients");

        migrationBuilder.DropTable(name: "Users");

        migrationBuilder.DropIndex(name: "IX_Bookings_PatientUserId", table: "Bookings");

        migrationBuilder.DropColumn(name: "PatientUserId", table: "Bookings");

        migrationBuilder.RenameColumn(name: "UserId", table: "Patients", newName: "Id");

        migrationBuilder.RenameColumn(name: "UserId", table: "Caregivers", newName: "Id");

        migrationBuilder.RenameColumn(
            name: "CaregiverUserId",
            table: "CaregiverDailySchedules",
            newName: "CaregiverId"
        );

        migrationBuilder.RenameIndex(
            name: "IX_CaregiverDailySchedules_CaregiverUserId",
            table: "CaregiverDailySchedules",
            newName: "IX_CaregiverDailySchedules_CaregiverId"
        );

        migrationBuilder.RenameColumn(name: "UserId1", table: "Bookings", newName: "PatientId");

        migrationBuilder.RenameIndex(
            name: "IX_Bookings_UserId1",
            table: "Bookings",
            newName: "IX_Bookings_PatientId"
        );

        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Patients",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "FirstName",
            table: "Patients",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "LastName",
            table: "Patients",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "PasswordHash",
            table: "Patients",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "Patients",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "Roles",
            table: "Patients",
            type: "jsonb",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "Username",
            table: "Patients",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Caregivers",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "FirstName",
            table: "Caregivers",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "LastName",
            table: "Caregivers",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "PasswordHash",
            table: "Caregivers",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "Caregivers",
            type: "text",
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddColumn<string>(
            name: "Roles",
            table: "Caregivers",
            type: "jsonb",
            nullable: false,
            defaultValue: "[]"
        );

        migrationBuilder.AddColumn<string>(
            name: "Username",
            table: "Caregivers",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: ""
        );

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Patients_PatientId",
            table: "Bookings",
            column: "PatientId",
            principalTable: "Patients",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade
        );

        migrationBuilder.AddForeignKey(
            name: "FK_CaregiverDailySchedules_Caregivers_CaregiverId",
            table: "CaregiverDailySchedules",
            column: "CaregiverId",
            principalTable: "Caregivers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade
        );
    }
}
