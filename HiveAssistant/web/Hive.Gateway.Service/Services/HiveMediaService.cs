using BeeHive.App.Hives.Repositories;
using BeeHive.App.Hives.Repositories.Specifications;
using BeeHive.Contract.Hives.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.App.DataAccess;

namespace Hive.Gateway.Service.Services;

public interface IHiveMediaService
{
    Task<IList<HiveMediaDto>> ListMedias(int hiveId, CancellationToken cancellationToken = default);

    Task<HiveMediaDto> SaveMedia(int hiveId, string name, Stream data, CancellationToken cancellationToken = default);
}

public class HiveMediaService(
    IHiveRepository hiveRepository,
    IHiveMediaRepository hiveMediaRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration) : IHiveMediaService
{
    public async Task<IList<HiveMediaDto>> ListMedias(int hiveId, CancellationToken cancellationToken = default)
    {
        var spec = new HiveMediaDtoSpecification()
        {
            HiveId = hiveId
        };

        return await hiveMediaRepository.GetAsync<HiveMediaDto>(spec, cancellationToken);
    }

    public async Task<HiveMediaDto> SaveMedia(int hiveId, string name, Stream data, CancellationToken cancellationToken = default)
    {
        try
        {
            using var ms = new MemoryStream();
            var copyTask = data.CopyToAsync(ms);

            var hive = await hiveRepository.GetByIdAsync(hiveId, cancellationToken);
            if (hive is null)
                throw new NullReferenceException(nameof(hive));

            var section = configuration.GetSection("UploadMediaClient:Cloudinary");
            var n = section["Name"];
            var k = section["Key"];
            var s = section["Secret"];

            var account = new Account(n, k, s);
            var cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;

            await copyTask;
            ms.Position = 0;
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name, ms)
            };

            var uploadResult = cloudinary.Upload(uploadParams);
            string imageUrl = uploadResult.SecureUrl.ToString();

            var media = hive.CreateMedia(imageUrl, null, name, BeeHive.Domain.Hives.MediaType.Photo);
            await hiveMediaRepository.AddAsync(media, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return media.ToDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}