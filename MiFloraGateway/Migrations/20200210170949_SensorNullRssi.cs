using Microsoft.EntityFrameworkCore.Migrations;

namespace MiFloraGateway.Migrations
{
    public partial class SensorNullRssi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rssi",
                table: "DeviceSensorDistances",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rssi",
                table: "DeviceSensorDistances",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
