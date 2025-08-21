using BeeHive.App;
using Microsoft.Extensions.Configuration;

namespace BeeHive.Infra.DataAccess;

internal class StorageManager(IConfiguration configuration) : IStorageManager
{
    public string GetAudioFilePath(string hiveKey, string fileName)
    {
        var dir = $"{configuration["ConnectionStrings:PcmFilesDirectory"]}\\{hiveKey}";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return $"{dir}\\{fileName}";
    }
}