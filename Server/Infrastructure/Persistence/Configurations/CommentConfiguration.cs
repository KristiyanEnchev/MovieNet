namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(c => c.Movie)
                    .WithMany(m => m.Comments)
                    .HasForeignKey(c => c.MovieId)
                    .HasPrincipalKey(m => m.TmdbId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}