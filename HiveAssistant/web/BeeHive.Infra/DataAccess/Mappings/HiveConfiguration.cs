using BeeHive.Domain.Hives;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.DataAccess.Mappings;

internal sealed class HiveConfiguration : EntityConfiguration<Hive, int>, IEntityTypeConfiguration<Hive>
{
    public void Configure(EntityTypeBuilder<Hive> builder)
    {
        ConfigureEntity(builder);
        ConfigureAudited<Hive, string>(builder);

        builder.Property(e => e.UniqueKey)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.AudioSensorSampleRate)
            .IsRequired();
        builder.Property(e => e.AudioSensorBitsPerSample)
            .IsRequired();
        builder.Property(e => e.AudioSensorChannels)
            .IsRequired();

        builder.HasOne(e => e.BeeGarden)
            .WithMany()
            .HasForeignKey(e => e.BeeGardenId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex(e => new { e.BeeGardenId, e.UniqueKey })
            .IsUnique(true);
    }
}