using ETicaretAPI.Common.Infrastructure.Mappings;
using ETicaretAPI.Services.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ETicaretAPI.Services.Catalog.Persistence.EntityTypeConfigurations;

public class CategoryMapping : BaseEntityConfiguration<Category>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
                .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(100)
                .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ImageUrl)
           .HasMaxLength(300);

        builder.Property(x => x.DisplayOrder)
           .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
        .HasDefaultValue(true);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasMany(x => x.SubCategories)
            .WithOne(x => x.ParentCategory)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}