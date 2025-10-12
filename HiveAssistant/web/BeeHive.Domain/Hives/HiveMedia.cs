using Core.Domain.Aggregates;

namespace BeeHive.Domain.Hives;

public sealed class HiveMedia : Entity<int>, ICreationAudited<string>, ISynchronizableEntity
{
    internal HiveMedia(Hive hive,
        string? url,
        string? localPath,
        string title,
        MediaType type)
    {
        Hive = hive;
        HiveId = hive.Id;
        Url = url;
        LocalPath = localPath;
        Title = title;
        MediaType = type;
    }

    public HiveMedia(Hive hive, int orginalId)
    {
        Hive = hive;
        OrginalId = orginalId;
        Title = string.Empty;
    }

    private HiveMedia(string title)
    {
        Hive = null!;
        Title = title;
    }

    public int HiveId { get; private set; }
    public Hive Hive { get; private set; }

    public string? Url { get; private set; }
    public string? LocalPath { get; private set; }
    public string Title { get; private set; }
    public MediaType MediaType { get; private set; }

    public DateTime CreatedDate { get; private set; }
    public string? CreatedBy { get; private set; }

    public int? OrginalId { get; private set; }
    public bool IsDeleted { get; private set; }

    public DateTime CreatedOrUpdatedDate { get; private set; }

    public void Update(string title)
    {
        Title = title;
    }

    public void Update(string? url,
        string? localPath,
        string title,
        MediaType type)
    {
        Url = url;
        LocalPath = localPath;
        Title = title;
        MediaType = type;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}