using BeeHive.Domain.Hives;

namespace BeeHive.Contract.Hives.Models;

public class HiveMediaDto
{
    public required int Id { get; init; }
    public required string? Url { get; init; }
    public required string? LocalPath { get; init; }
    public required string Title { get; set; }
    public required MediaType MediaType { get; init; }
}