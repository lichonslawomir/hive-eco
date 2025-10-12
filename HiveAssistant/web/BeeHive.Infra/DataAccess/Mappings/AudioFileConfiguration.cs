using BeeHive.Domain.Hives.Audio;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.DataAccess.Mappings;

internal sealed class AudioFileConfiguration : IEntityTypeConfiguration<AudioFile>
{
    public void Configure(EntityTypeBuilder<AudioFile> builder)
    {
        builder.HasKey(e => new { e.HiveId, e.Timestamp });

        builder.HasOne(e => e.Hive)
            .WithMany()
            .HasForeignKey(e => e.HiveId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.Property(e => e.Timestamp)
            .UtcDateTime()
            .IsRequired();

        builder.Property(e => e.SampleRate)
            .IsRequired();
        builder.Property(e => e.BitsPerSample)
            .IsRequired();
        builder.Property(e => e.Channels)
            .IsRequired();

        builder.Property(e => e.Complete)
            .IsRequired();

        builder.Property(e => e.DurationSec)
            .IsRequired();
        builder.Property(e => e.Frequency)
            .IsRequired();
        builder.Property(e => e.AmplitudeRms)
            .IsRequired();
        builder.Property(e => e.AmplitudeMav)
            .IsRequired();

        builder.HasIndex(e => new { e.HiveId, e.Complete })
            .IsUnique(false);
    }
}