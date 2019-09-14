using Microsoft.EntityFrameworkCore.Migrations;

namespace MiFloraGateway.Migrations
{
    public partial class FixedMacAddressLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Sensors",
                fixedLength: true,
                maxLength: 17,
                nullable: false,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Devices",
                fixedLength: true,
                maxLength: 17,
                nullable: false,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 18);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Sensors",
                fixedLength: true,
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 17);

            migrationBuilder.AlterColumn<string>(
                name: "MACAddress",
                table: "Devices",
                fixedLength: true,
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 17);
        }
    }
}
