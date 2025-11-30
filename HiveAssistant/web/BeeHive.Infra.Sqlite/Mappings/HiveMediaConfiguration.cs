using BeeHive.Domain.Hives;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.Sqlite.Mappings;

internal sealed class HiveMediaConfiguration : EntityConfiguration<HiveMedia, int>, IEntityTypeConfiguration<HiveMedia>
{
    public void Configure(EntityTypeBuilder<HiveMedia> builder)
    {
        base.ConfigureEntity(builder);
        base.ConfigureCreationAudited<HiveMedia, string>(builder);
        builder.ConfigureSynchronizable();

        builder.HasOne(e => e.Hive)
            .WithMany(e => e.Media)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(e => e.HiveId)
            .IsRequired(true);

        builder.HasIndex(e => new { e.HiveId, e.OrginalId });
    }
}