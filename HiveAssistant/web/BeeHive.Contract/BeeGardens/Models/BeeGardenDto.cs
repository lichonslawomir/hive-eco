namespace BeeHive.Contract.BeeGardens.Models;

public struct BeeGardenDto
{
    public required int Id { get; init; }
    public required string UniqueKey { get; init; }
    public required string Name { get; init; }
    public required string TimeZone { get; init; }
}