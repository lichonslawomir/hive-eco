namespace BeeHive.Contract.Holdings.Models;

public struct HoldingDto
{
    public required int Id { get; init; }
    public required string UniqueKey { get; init; }
    public required string Name { get; init; }
}