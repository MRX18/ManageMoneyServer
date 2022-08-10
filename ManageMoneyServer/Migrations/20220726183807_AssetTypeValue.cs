using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageMoneyServer.Migrations
{
    public partial class AssetTypeValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "AssetTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "AssetTypes");
        }
    }
}
