using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeHive.Infra.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Exportable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOrUpdatedDate",
                table: "TimeAggregateSeriesData",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "TimeAggregateSeriesData",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOrUpdatedDate",
                table: "Hives",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BeeGardenImportStates",
                columns: table => new
                {
                    BeeGardenId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExportEntity = table.Column<int>(type: "INTEGER", nullable: false),
                    LastCreatedOrUpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeeGardenImportStates", x => new { x.BeeGardenId, x.ExportEntity });
                    table.ForeignKey(
                        name: "FK_BeeGardenImportStates_BeeGardens_BeeGardenId",
                        column: x => x.BeeGardenId,
                        principalTable: "BeeGardens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HiveMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HiveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    LocalPath = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    MediaType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    OrginalId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOrUpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Version = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiveMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiveMedia_Hives_HiveId",
                        column: x => x.HiveId,
                        principalTable: "Hives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeAggregateSeriesData_CreatedOrUpdatedDate",
                table: "TimeAggregateSeriesData",
                column: "CreatedOrUpdatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Hives_CreatedOrUpdatedDate",
                table: "Hives",
                column: "CreatedOrUpdatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_HiveMedia_CreatedOrUpdatedDate",
                table: "HiveMedia",
                column: "CreatedOrUpdatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_HiveMedia_HiveId_OrginalId",
                table: "HiveMedia",
                columns: new[] { "HiveId", "OrginalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeeGardenImportStates");

            migrationBuilder.DropTable(
                name: "HiveMedia");

            migrationBuilder.DropIndex(
                name: "IX_TimeAggregateSeriesData_CreatedOrUpdatedDate",
                table: "TimeAggregateSeriesData");

            migrationBuilder.DropIndex(
                name: "IX_Hives_CreatedOrUpdatedDate",
                table: "Hives");

            migrationBuilder.DropColumn(
                name: "CreatedOrUpdatedDate",
                table: "TimeAggregateSeriesData");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "TimeAggregateSeriesData");

            migrationBuilder.DropColumn(
                name: "CreatedOrUpdatedDate",
                table: "Hives");
        }
    }
}
