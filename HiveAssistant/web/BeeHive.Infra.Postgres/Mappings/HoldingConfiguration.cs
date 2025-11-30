using BeeHive.Domain.Holdings;
using Core.Infra.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeeHive.Infra.Sqlite.Mappings;

internal sealed class HoldingConfiguration : EntityConfiguration<Holding, int>, IEntityTypeConfiguration<Holding>
{
    public void Configure(EntityTypeBuilder<Holding> builder)
    {
        ConfigureEntity(builder);
        ConfigureAudited<Holding, string>(builder);

        builder.Property(e => e.UniqueKey)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(e => e.UniqueKey)
            .IsUnique(true);
    }
}