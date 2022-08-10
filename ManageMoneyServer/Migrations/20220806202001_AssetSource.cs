using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageMoneyServer.Migrations
{
    public partial class AssetSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Sources_SourceId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_SourceId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Assets");

            migrationBuilder.CreateTable(
                name: "AssetSource",
                columns: table => new
                {
                    AssetsAssetId = table.Column<int>(type: "int", nullable: false),
                    SourcesSourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSource", x => new { x.AssetsAssetId, x.SourcesSourceId });
                    table.ForeignKey(
                        name: "FK_AssetSource_Assets_AssetsAssetId",
                        column: x => x.AssetsAssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetSource_Sources_SourcesSourceId",
                        column: x => x.SourcesSourceId,
                        principalTable: "Sources",
                        principalColumn: "SourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetSource_SourcesSourceId",
                table: "AssetSource",
                column: "SourcesSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSource");

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SourceId",
                table: "Assets",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Sources_SourceId",
                table: "Assets",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "SourceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
