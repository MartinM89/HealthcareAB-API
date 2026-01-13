using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareAB_v1.Migrations;

/// <inheritdoc />
public partial class RemovedTimeLengthFromTimeSlotModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "TimeLength", table: "TimeSlots");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "TimeLength",
            table: "TimeSlots",
            type: "double precision",
            nullable: false,
            defaultValue: 0.0
        );
    }
}
