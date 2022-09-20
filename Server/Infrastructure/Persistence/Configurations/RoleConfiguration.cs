namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class RoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("Roles", "Identity");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Description).HasMaxLength(256);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(256);
            builder.Property(r => r.NormalizedName).IsRequired().HasMaxLength(256);

            builder.Property(r => r.CreatedBy).HasMaxLength(256);
            builder.Property(r => r.CreatedDate);
            builder.Property(r => r.UpdatedBy).HasMaxLength(256);
            builder.Property(r => r.UpdatedDate);

            builder.Ignore(r => r.DomainEvents);
        }
    }
}