namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class UserMovieInteractionConfiguration : IEntityTypeConfiguration<UserMovieInteraction>
    {
        public void Configure(EntityTypeBuilder<UserMovieInteraction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.UserId, x.MovieId })
                   .IsUnique();

            builder.HasOne(x => x.Movie)
                   .WithMany(m => m.UserInteractions)
                   .HasForeignKey(x => x.MovieId)
                   .HasPrincipalKey(m => m.TmdbId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                   .WithMany(u => u.MovieInteractions)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}