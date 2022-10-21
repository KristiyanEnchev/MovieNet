namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Id)
                .IsRequired();

            builder.Property(g => g.TmdbId)
                .IsRequired();

            builder.HasIndex(g => g.TmdbId)
                .IsUnique()
                .HasDatabaseName("ix_genres_tmdb_id");

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.MediaType)
                .IsRequired();

            builder.HasMany(g => g.Movies);
        }
    }
}
