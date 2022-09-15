namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);
            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        }
    }
}