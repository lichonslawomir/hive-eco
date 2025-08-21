namespace BeeHive.App;

public interface IStorageManager
{
    public string GetAudioFilePath(string hiveKey, string fileName);
}