using BeeHive.Contract.Hives.Models;

namespace BeeHive.Contract.Interfaces;

public interface IHiveMediaService
{
    Task<IList<HiveMediaDto>> ListMedias(int hiveId, CancellationToken cancellationToken = default);

    Task<HiveMediaDto> SaveMedia(int hiveId, string name, Stream data, CancellationToken cancellationToken = default);
}
