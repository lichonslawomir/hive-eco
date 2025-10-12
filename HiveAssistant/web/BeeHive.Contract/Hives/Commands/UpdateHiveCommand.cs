using BeeHive.Contract.Hives.Models;
using Core.Contract;

namespace BeeHive.Contract.Hives.Commands;

public struct UpdateHiveCommand : ICommand
{
    public required int Id { get; set; }

    public required HiveUpdateModel Data { get; set; }
}