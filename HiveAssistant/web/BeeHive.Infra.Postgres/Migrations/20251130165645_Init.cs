using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeeHive.Infra.Postgres.Migrations
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holdings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BeeGardens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HoldingId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
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
                    BeeGardenId = table.Column<int>(type: "integer", nullable: false),
                    ExportEntity = table.Column<int>(type: "integer", nullable: false),
                    LastCreatedOrUpdatedDate = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BeeGardenId = table.Column<int>(type: "integer", nullable: false),
                    ComPort = table.Column<string>(type: "text", nullable: true),
                    LastComPortUsed = table.Column<string>(type: "text", nullable: true),
                    GraphColor = table.Column<int>(type: "integer", nullable: false),
                    SerialBound = table.Column<int>(type: "integer", nullable: false),
                    AudioSensorSampleRate = table.Column<int>(type: "integer", nullable: false),
                    AudioSensorChannels = table.Column<int>(type: "integer", nullable: false),
                    AudioSensorBitsPerSample = table.Column<int>(type: "integer", nullable: false),
                    CreatedOrUpdatedDate = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
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
                    HiveId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    SampleRate = table.Column<int>(type: "integer", nullable: false),
                    Channels = table.Column<int>(type: "integer", nullable: false),
                    BitsPerSample = table.Column<int>(type: "integer", nullable: false),
                    Complete = table.Column<bool>(type: "boolean", nullable: false),
                    DurationSec = table.Column<float>(type: "real", nullable: false),
                    Frequency = table.Column<float>(type: "real", nullable: false),
                    AmplitudePeak = table.Column<float>(type: "real", nullable: false),
                    AmplitudeRms = table.Column<float>(type: "real", nullable: false),
                    AmplitudeMav = table.Column<float>(type: "real", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HiveId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    LocalPath = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    OrginalId = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOrUpdatedDate = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    HiveId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    LasteAggregateTime = table.Column<string>(type: "text", nullable: true),
                    TimeSeriesId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
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
                    TimeSeriesId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false)
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
                    TimeAggregateSeriesId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<string>(type: "text", nullable: false),
                    DurationSec = table.Column<float>(type: "real", nullable: false),
                    Frequency = table.Column<float>(type: "real", nullable: false),
                    AmplitudePeak = table.Column<float>(type: "real", nullable: false),
                    AmplitudeRms = table.Column<float>(type: "real", nullable: false),
                    AmplitudeMav = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioAggregateStatsData", x => new { x.TimeAggregateSeriesId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_AudioAggregateStatsData_TimeAggregateSeries_TimeAggregateSe~",
                        column: x => x.TimeAggregateSeriesId,
                        principalTable: "TimeAggregateSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeAggregateSeriesData",
                columns: table => new
                {
                    TimeAggregateSeriesId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    MaxValue = table.Column<float>(type: "real", nullable: true),
                    MinValue = table.Column<float>(type: "real", nullable: true),
                    AvgValue = table.Column<float>(type: "real", nullable: true),
                    MedValue = table.Column<float>(type: "real", nullable: true),
                    CreatedOrUpdatedDate = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeAggregateSeriesData", x => new { x.TimeAggregateSeriesId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_TimeAggregateSeriesData_TimeAggregateSeries_TimeAggregateSe~",
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
