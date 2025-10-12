using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeeHive.Infra.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holdings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UniqueKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Version = table.Column<uint>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedDate = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holdings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeeGardens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UniqueKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    TimeZone = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    HoldingId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<uint>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedDate = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeeGardens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeeGardens_Holdings_HoldingId",
                        column: x => x.HoldingId,
                        principalTable: "Holdings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeeGardenImportStates",
                columns: table => new
                {
                    BeeGardenId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExportEntity = table.Column<int>(type: "INTEGER", nullable: false),
                    LastCreatedOrUpdatedDate = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "Hives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    UniqueKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BeeGardenId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComPort = table.Column<string>(type: "TEXT", nullable: true),
                    LastComPortUsed = table.Column<string>(type: "TEXT", nullable: true),
                    AudioSensorSampleRate = table.Column<int>(type: "INTEGER", nullable: false),
                    AudioSensorChannels = table.Column<int>(type: "INTEGER", nullable: false),
                    AudioSensorBitsPerSample = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOrUpdatedDate = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<uint>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedDate = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hives_BeeGardens_BeeGardenId",
                        column: x => x.BeeGardenId,
                        principalTable: "BeeGardens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioFiles",
                columns: table => new
                {
                    HiveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    SampleRate = table.Column<int>(type: "INTEGER", nullable: false),
                    Channels = table.Column<int>(type: "INTEGER", nullable: false),
                    BitsPerSample = table.Column<int>(type: "INTEGER", nullable: false),
                    Complete = table.Column<bool>(type: "INTEGER", nullable: false),
                    DurationSec = table.Column<float>(type: "REAL", nullable: false),
                    Frequency = table.Column<float>(type: "REAL", nullable: false),
                    AmplitudePeak = table.Column<float>(type: "REAL", nullable: false),
                    AmplitudeRms = table.Column<float>(type: "REAL", nullable: false),
                    AmplitudeMav = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioFiles", x => new { x.HiveId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_AudioFiles_Hives_HiveId",
                        column: x => x.HiveId,
                        principalTable: "Hives",
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
                    CreatedDate = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    OrginalId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOrUpdatedDate = table.Column<string>(type: "TEXT", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "TimeSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    HiveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSeries_Hives_HiveId",
                        column: x => x.HiveId,
                        principalTable: "Hives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeAggregateSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Period = table.Column<int>(type: "INTEGER", nullable: false),
                    LasteAggregateTime = table.Column<string>(type: "TEXT", nullable: true),
                    TimeSeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAggregateSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeAggregateSeries_TimeSeries_TimeSeriesId",
                        column: x => x.TimeSeriesId,
                        principalTable: "TimeSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSeriesData",
                columns: table => new
                {
                    TimeSeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSeriesData", x => new { x.TimeSeriesId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_TimeSeriesData_TimeSeries_TimeSeriesId",
                        column: x => x.TimeSeriesId,
                        principalTable: "TimeSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioAggregateStatsData",
                columns: table => new
                {
                    TimeAggregateSeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<string>(type: "TEXT", nullable: false),
                    DurationSec = table.Column<float>(type: "REAL", nullable: false),
                    Frequency = table.Column<float>(type: "REAL", nullable: false),
                    AmplitudePeak = table.Column<float>(type: "REAL", nullable: false),
                    AmplitudeRms = table.Column<float>(type: "REAL", nullable: false),
                    AmplitudeMav = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioAggregateStatsData", x => new { x.TimeAggregateSeriesId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_AudioAggregateStatsData_TimeAggregateSeries_TimeAggregateSeriesId",
                        column: x => x.TimeAggregateSeriesId,
                        principalTable: "TimeAggregateSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeAggregateSeriesData",
                columns: table => new
                {
                    TimeAggregateSeriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<string>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxValue = table.Column<float>(type: "REAL", nullable: true),
                    MinValue = table.Column<float>(type: "REAL", nullable: true),
                    AvgValue = table.Column<float>(type: "REAL", nullable: true),
                    MedValue = table.Column<float>(type: "REAL", nullable: true),
                    CreatedOrUpdatedDate = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAggregateSeriesData", x => new { x.TimeAggregateSeriesId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_TimeAggregateSeriesData_TimeAggregateSeries_TimeAggregateSeriesId",
                        column: x => x.TimeAggregateSeriesId,
                        principalTable: "TimeAggregateSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_HiveId_Complete",
                table: "AudioFiles",
                columns: new[] { "HiveId", "Complete" });

            migrationBuilder.CreateIndex(
                name: "IX_BeeGardens_HoldingId_UniqueKey",
                table: "BeeGardens",
                columns: new[] { "HoldingId", "UniqueKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HiveMedia_CreatedOrUpdatedDate",
                table: "HiveMedia",
                column: "CreatedOrUpdatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_HiveMedia_HiveId_OrginalId",
                table: "HiveMedia",
                columns: new[] { "HiveId", "OrginalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Hives_BeeGardenId_UniqueKey",
                table: "Hives",
                columns: new[] { "BeeGardenId", "UniqueKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hives_CreatedOrUpdatedDate",
                table: "Hives",
                column: "CreatedOrUpdatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_UniqueKey",
                table: "Holdings",
                column: "UniqueKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeAggregateSeries_TimeSeriesId_Period",
                table: "TimeAggregateSeries",
                columns: new[] { "TimeSeriesId", "Period" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeAggregateSeriesData_CreatedOrUpdatedDate",
                table: "TimeAggregateSeriesData",
                column: "CreatedOrUpdatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSeries_HiveId_Kind",
                table: "TimeSeries",
                columns: new[] { "HiveId", "Kind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioAggregateStatsData");

            migrationBuilder.DropTable(
                name: "AudioFiles");

            migrationBuilder.DropTable(
                name: "BeeGardenImportStates");

            migrationBuilder.DropTable(
                name: "HiveMedia");

            migrationBuilder.DropTable(
                name: "TimeAggregateSeriesData");

            migrationBuilder.DropTable(
                name: "TimeSeriesData");

            migrationBuilder.DropTable(
                name: "TimeAggregateSeries");

            migrationBuilder.DropTable(
                name: "TimeSeries");

            migrationBuilder.DropTable(
                name: "Hives");

            migrationBuilder.DropTable(
                name: "BeeGardens");

            migrationBuilder.DropTable(
                name: "Holdings");
        }
    }
}
