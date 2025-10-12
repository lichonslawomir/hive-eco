using BeeHive.Domain.Hives;

namespace BeeHive.Contract.Export;

public struct HiveMediaExportModel
{
    public required string HoldingUniqueKey { get; init; }
    public required string BeeGardenUniqueKey { get; init; }
    public required string HiveUniqueKey { get; init; }

    public required int Id { get; init; }
    public required bool IsDeleted { get; init; }

    public required string? Url { get; init; }
    public required string? LocalPath { get; init; }
    public required string Title { get; init; }
    public required MediaType MediaType { get; init; }

    public required DateTimeOffset CreatedDate { get; init; }
    public required string? CreatedBy { get; init; }

    public required DateTimeOffset CreatedOrUpdatedDate { get; init; }
}