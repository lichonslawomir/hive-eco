using Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infra.DataAccess.Mappings;

public class EntityConfiguration<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    public void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Version)
            .IsConcurrencyToken();
    }

    public void ConfigureEntityWithRowVersion(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Version)
            .IsRowVersion();
    }

    protected void ConfigureCreationAudited<TCreationAudited, TUserId>(EntityTypeBuilder<TCreationAudited> builder)
        where TCreationAudited : class, ICreationAudited<TUserId>
    {
        builder.Property(e => e.CreatedBy);

        builder.Property(e => e.CreatedDate)
            .UtcDateTime()
            .IsRequired();
    }

    protected void ConfigureUpdateAudited<TAuditableEntity, TUserId>(EntityTypeBuilder<TAuditableEntity> builder)
        where TAuditableEntity : class, IAuditableEntity<TUserId>
    {
        builder.Property(e => e.UpdatedBy);

        builder.Property(e => e.UpdatedDate)
            .UtcDateTime();
    }

    protected void ConfigureAudited<TAuditableEntity, TUserId>(EntityTypeBuilder<TAuditableEntity> builder)
        where TAuditableEntity : class, IAuditableEntity<TId, TUserId>
    {
        ConfigureCreationAudited<TAuditableEntity, TUserId>(builder);
        ConfigureUpdateAudited<TAuditableEntity, TUserId>(builder);
    }
}

public static class EntityConfigurationExtension
{
    public static void ConfigureSynchronizable<TSynchronizableEntity>(this EntityTypeBuilder<TSynchronizableEntity> builder)
        where TSynchronizableEntity : class, ISynchronizableEntity
    {
        builder.Property(e => e.CreatedOrUpdatedDate)
            .UtcDateTime()
            .IsRequired();

        builder.HasIndex(e => e.CreatedOrUpdatedDate);
    }

    public static PropertyBuilder<DateTime> UtcDateTime(this PropertyBuilder<DateTime> builder)
    {
        return builder.HasConversion(
            v => v.ToString("o"),
            v => DateTime.Parse(v, null, System.Globalization.DateTimeStyles.RoundtripKind)
         );
    }

    public static PropertyBuilder<DateTime?> UtcDateTime(this PropertyBuilder<DateTime?> builder)
    {
        return builder.HasConversion(
            v => v.HasValue ? v.Value.ToString("o") : null,
            v => string.IsNullOrEmpty(v) ? null : DateTime.Parse(v, null, System.Globalization.DateTimeStyles.RoundtripKind)
         );
    }
}