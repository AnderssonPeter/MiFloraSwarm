using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MiFloraGateway.Migrations
{
    public partial class convertFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceSensorDistances",
                table: "DeviceSensorDistances");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Sensors");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "SensorBatteryReadings",
                maxLength: 12,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "When",
                table: "DeviceSensorDistances",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceSensorDistances",
                table: "DeviceSensorDistances",
                columns: new[] { "DeviceId", "SensorId", "When" });

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    DeviceId = table.Column<int>(nullable: false),
                    When = table.Column<DateTime>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    SensorId = table.Column<int>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    Event = table.Column<string>(maxLength: 21, nullable: false),
                    Result = table.Column<string>(maxLength: 10, nullable: false),
                    Message = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => new { x.DeviceId, x.When });
                    table.ForeignKey(
                        name: "FK_LogEntries_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogEntries_Sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_SensorId",
                table: "LogEntries",
                column: "SensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeviceSensorDistances",
                table: "DeviceSensorDistances");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SensorBatteryReadings");

            migrationBuilder.DropColumn(
                name: "When",
                table: "DeviceSensorDistances");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Sensors",
                maxLength: 12,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeviceSensorDistances",
                table: "DeviceSensorDistances",
                columns: new[] { "DeviceId", "SensorId" });
        }
    }
}
