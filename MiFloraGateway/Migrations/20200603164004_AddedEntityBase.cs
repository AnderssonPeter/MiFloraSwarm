using Microsoft.EntityFrameworkCore.Migrations;

namespace MiFloraGateway.Migrations
{
    public partial class AddedEntityBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plants_LatinName",
                table: "Plants");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sensors",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LatinName",
                table: "Plants",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Plants",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Display",
                table: "Plants",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "Plants",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "LogEntries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LogEntries",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Devices",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_LatinName",
                table: "Plants",
                column: "LatinName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_PlantId",
                table: "LogEntries",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_UserId",
                table: "LogEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_IPAddress",
                table: "Devices",
                column: "IPAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Name",
                table: "Devices",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_Plants_PlantId",
                table: "LogEntries",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_AspNetUsers_UserId",
                table: "LogEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_Plants_PlantId",
                table: "LogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_AspNetUsers_UserId",
                table: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_Plants_LatinName",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_LogEntries_PlantId",
                table: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_LogEntries_UserId",
                table: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_Devices_IPAddress",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_Name",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LogEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sensors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LatinName",
                table: "Plants",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Display",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Alias",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Plants_LatinName",
                table: "Plants",
                column: "LatinName",
                unique: true,
                filter: "[LatinName] IS NOT NULL");
        }
    }
}
