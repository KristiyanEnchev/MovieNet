namespace Application.Common.Mappings
{
    using AutoMapper;

    using Domain.Entities;

    using Models.Tmdb;
    using Models.Movie;
    using Models.Comments;

    public class MovieMappingProfile : Profile
    {
        public MovieMappingProfile()
        {
            // Domain -> DTO mappings
            CreateMap<Movie, MovieDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.TmdbId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                // These fields don't exist in domain entity, so ignore them
                .ForMember(d => d.Overview, opt => opt.Ignore())
                .ForMember(d => d.PosterPath, opt => opt.Ignore())
                .ForMember(d => d.BackdropPath, opt => opt.Ignore())
                .ForMember(d => d.MediaType, opt => opt.Ignore())
                .ForMember(d => d.ReleaseDate, opt => opt.Ignore())
                .ForMember(d => d.VoteAverage, opt => opt.Ignore())
                .ForMember(d => d.VoteCount, opt => opt.Ignore())
                .ForMember(d => d.Popularity, opt => opt.Ignore())
                .ForMember(d => d.OriginalTitle, opt => opt.Ignore())
                .ForMember(d => d.IsLiked, opt => opt.Ignore())
                .ForMember(d => d.IsDisliked, opt => opt.Ignore())
                .ForMember(d => d.IsWatchlisted, opt => opt.Ignore());

            // TMDB -> DTO mappings
            CreateMap<TmdbMovieDto, MovieDto>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.TmdbId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.GetTitle()))
                .ForMember(d => d.OriginalTitle, opt => opt.MapFrom(s => s.GetOriginalTitle()))
                .ForMember(d => d.Overview, opt => opt.MapFrom(s => s.Overview))
                .ForMember(d => d.PosterPath, opt => opt.MapFrom(s => s.PosterPath))
                .ForMember(d => d.BackdropPath, opt => opt.MapFrom(s => s.BackdropPath))
                .ForMember(d => d.MediaType, opt => opt.MapFrom(s => s.MediaType))
                .ForMember(d => d.ReleaseDate, opt => opt.MapFrom(s => s.GetReleaseDate()))
                .ForMember(d => d.VoteAverage, opt => opt.MapFrom(s => s.VoteAverage))
                .ForMember(d => d.VoteCount, opt => opt.MapFrom(s => s.VoteCount))
                .ForMember(d => d.Popularity, opt => opt.MapFrom(s => s.Popularity))
                .ForMember(d => d.IsLiked, opt => opt.Ignore())
                .ForMember(d => d.IsDisliked, opt => opt.Ignore())
                .ForMember(d => d.IsWatchlisted, opt => opt.Ignore());

            CreateMap<TmdbMovieDetailsDto, MovieDetailsDto>()
                .IncludeBase<TmdbMovieDto, MovieDto>()
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.TmdbId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.GetTitle()))
                .ForMember(d => d.OriginalTitle, opt => opt.MapFrom(s => s.GetOriginalTitle()))
                .ForMember(d => d.Overview, opt => opt.MapFrom(s => s.Overview))
                .ForMember(d => d.PosterPath, opt => opt.MapFrom(s => s.PosterPath))
                .ForMember(d => d.BackdropPath, opt => opt.MapFrom(s => s.BackdropPath))
                .ForMember(d => d.MediaType, opt => opt.MapFrom(s => s.MediaType))
                .ForMember(d => d.ReleaseDate, opt => opt.MapFrom(s => s.GetReleaseDate()))
                .ForMember(d => d.Runtime, opt => opt.MapFrom(s => s.Runtime))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
                .ForMember(d => d.Tagline, opt => opt.MapFrom(s => s.Tagline))
                .ForMember(d => d.Homepage, opt => opt.MapFrom(s => s.Homepage))
                .ForMember(d => d.Budget, opt => opt.MapFrom(s => s.Budget))
                .ForMember(d => d.Revenue, opt => opt.MapFrom(s => s.Revenue))
                .ForMember(d => d.Genres, opt => opt.MapFrom(s => s.Genres))
                .ForMember(d => d.Cast, opt => opt.MapFrom(s => s.Cast))
                .ForMember(d => d.Videos, opt => opt.MapFrom(s => s.Videos!.Results));

            // Supporting entity mappings
            CreateMap<Genre, GenreDto>()
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.TmdbId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));

            CreateMap<TmdbGenreDto, GenreDto>()
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));

            // Cast mapping
            CreateMap<TmdbCastDto, CastDto>()
                .ForMember(d => d.Id, opt => opt.Ignore())  
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Character, opt => opt.MapFrom(s => s.Character))
                .ForMember(d => d.ProfilePath, opt => opt.MapFrom(s => s.ProfilePath))
                .ForMember(d => d.Order, opt => opt.MapFrom(s => s.Order))
                .ForMember(d => d.Department, opt => opt.Ignore()) 
                .ForMember(d => d.Popularity, opt => opt.Ignore());

            // Video mapping
            CreateMap<TmdbVideoDto, VideoDto>()
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.TmdbId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Key, opt => opt.MapFrom(s => s.Key))
                .ForMember(d => d.Site, opt => opt.MapFrom(s => s.Site))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
                .ForMember(d => d.Size, opt => opt.Ignore())
                .ForMember(d => d.Official, opt => opt.Ignore())
                .ForMember(d => d.PublishedAt, opt => opt.Ignore());

            CreateMap<UserMovieInteraction, UserMovieInteractionDto>()
                .ForMember(d => d.MovieId, opt => opt.MapFrom(s => s.MovieId))
                .ForMember(d => d.IsLiked, opt => opt.MapFrom(s => s.IsLiked))
                .ForMember(d => d.IsDisliked, opt => opt.MapFrom(s => s.IsDisliked))
                .ForMember(d => d.IsWatchlisted, opt => opt.MapFrom(s => s.IsWatchlisted))
                .ForMember(d => d.Comments, opt => opt.Ignore());

            CreateMap<UserMovieInteractionDto, MovieDto>()
                .ForMember(d => d.TmdbId, opt => opt.MapFrom(s => s.MovieId))
                .ForMember(d => d.IsLiked, opt => opt.MapFrom(s => s.IsLiked))
                .ForMember(d => d.IsDisliked, opt => opt.MapFrom(s => s.IsDisliked))
                .ForMember(d => d.IsWatchlisted, opt => opt.MapFrom(s => s.IsWatchlisted))

                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Title, opt => opt.Ignore())
                .ForMember(d => d.OriginalTitle, opt => opt.Ignore())
                .ForMember(d => d.Overview, opt => opt.Ignore())
                .ForMember(d => d.PosterPath, opt => opt.Ignore())
                .ForMember(d => d.BackdropPath, opt => opt.Ignore())
                .ForMember(d => d.MediaType, opt => opt.Ignore())
                .ForMember(d => d.ReleaseDate, opt => opt.Ignore())
                .ForMember(d => d.VoteAverage, opt => opt.Ignore())
                .ForMember(d => d.VoteCount, opt => opt.Ignore())
                .ForMember(d => d.Popularity, opt => opt.Ignore());

            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Content, opt => opt.MapFrom(s => s.Content))
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId))
                .ForMember(d => d.MovieId, opt => opt.MapFrom(s => s.MovieId))
                .ForMember(d => d.Created, opt => opt.MapFrom(s => s.CreatedDate))
                .ForMember(d => d.Modified, opt => opt.MapFrom(s => s.UpdatedDate))
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName));

        }
    }
}