using Core.Contract;

namespace BeeHive.Contract.Hives.Commands;

public struct DeleteHiveMediaCommand : ICommand
{
    public required int Id { get; set; }
}