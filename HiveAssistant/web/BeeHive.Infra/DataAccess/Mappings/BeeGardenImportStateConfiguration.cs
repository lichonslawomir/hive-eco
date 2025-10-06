using BeeHive.Domain.BeeGardens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.DataAccess.Mappings;

internal sealed class BeeGardenImportStateConfiguration : IEntityTypeConfiguration<BeeGardenImportState>
{
    public void Configure(EntityTypeBuilder<BeeGardenImportState> builder)
    {
        builder.HasKey(x => new { x.BeeGardenId, x.ExportEntity });

        builder.HasOne(e => e.BeeGarden)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.BeeGardenId)
            .IsRequired(true);
    }
}