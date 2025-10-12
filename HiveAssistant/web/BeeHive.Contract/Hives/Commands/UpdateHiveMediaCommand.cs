using BeeHive.Contract.Hives.Models;
using Core.Contract;

namespace BeeHive.Contract.Hives.Commands;

public struct UpdateHiveMediaCommand : ICommand
{
    public required int Id { get; set; }

    public required HiveMediaUpdateModel Data { get; set; }
}