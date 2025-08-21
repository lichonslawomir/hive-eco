namespace BeeHive.Contract.Hives;

public class HiveDto
{
    public required int Id { get; init; }
    public required string UniqueKey { get; init; }
    public required string Name { get; init; }
}