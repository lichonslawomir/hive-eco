using BeeHive.Domain.BeeGardens;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.Sqlite.Mappings;

internal sealed class BeeGardenImportStateConfiguration : IEntityTypeConfiguration<BeeGardenImportState>
{
    public void Configure(EntityTypeBuilder<BeeGardenImportState> builder)
    {
        builder.HasKey(x => new { x.BeeGardenId, x.ExportEntity });

        builder.Property(e => e.LastCreatedOrUpdatedDate)
            .UtcDateTime()
            .IsRequired();

        builder.HasOne(e => e.BeeGarden)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.BeeGardenId)
            .IsRequired(true);
    }
}