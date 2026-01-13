using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations;

/// <inheritdoc />
public partial class ChangedHowEndPropertyInTimeSlotModelIsCalculated : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<TimeOnly>(
            name: "End",
            table: "TimeSlots",
            type: "time without time zone",
            nullable: false,
            defaultValue: new TimeOnly(0, 0, 0)
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "End", table: "TimeSlots");
    }
}
