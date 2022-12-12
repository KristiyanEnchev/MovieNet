import { createApi } from '@reduxjs/toolkit/query/react';
import { createBaseQueryWithReauth } from '../auth/baseQuery';
import { clearCredentials, updateToken } from '../auth/authActions';

const baseQueryWithReauth = createBaseQueryWithReauth({
  clearCredentials,
  updateToken,
});

const transformMediaResponse = (response) => {
  if (!response || !response.data) return [];
  return response.data.map((item) => ({
    id: item.id,
    tmdbId: item.tmdbId,
    mediaType: item.mediaType,
    title: item.title || item.name,
    posterPath: item.posterPath,
    backdropPath: item.backdropPath,
    voteAverage: item.voteAverage || 0,
    releaseDate: item.releaseDate || item.firstAirDate,
    overview: item.overview,
    isLiked: item.isLiked || false,
    isInWatchlist: item.isWatchlisted || false,
  }));
};

export const mediaApi = createApi({
  reducerPath: 'mediaApi',
  baseQuery: baseQueryWithReauth,
  tagTypes: [
    'UserInteraction',
    'Watchlist',
    'MovieDetails',
    'MediaList',
    'Genres',
    'Media',
  ],
  endpoints: (builder) => ({
    getTrendingMovies: builder.query({
      query: ({ mediaType = 'movie', timeWindow = 'week' } = {}) => ({
        url: '/movies/trending',
        params: { mediaType, timeWindow },
      }),
      transformResponse: (response) => {
        if (!response?.data) return [];
        return response.data;
      },
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ tmdbId }) => ({
                type: 'Media',
                id: tmdbId,
              })),
              { type: 'MediaList' },
            ]
          : [{ type: 'MediaList' }],
    }),

    getTrendingShows: builder.query({
      query: ({ mediaType = 'tv', timeWindow = 'week' } = {}) => ({
        url: '/movies/trending',
        params: { mediaType, timeWindow },
      }),
      transformResponse: (response) => {
        if (!response?.data) return [];
        return response.data;
      },
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ tmdbId }) => ({
                type: 'Media',
                id: tmdbId,
              })),
              { type: 'MediaList' },
            ]
          : [{ type: 'MediaList' }],
    }),

    searchMedia: builder.query({
      query: ({ query, page = 1, mediaType = 'multi' }) => ({
        url: '/movies/search',
        params: { query, page, mediaType },
      }),
      transformResponse: transformMediaResponse,
      providesTags: (result) =>
        result?.items
          ? [
              ...result.items.map(({ id }) => ({
                type: 'Media',
                id: id,
              })),
              { type: 'MediaList', id: 'SEARCH' },
            ]
          : [{ type: 'MediaList', id: 'SEARCH' }],
    }),

    getMovieDetails: builder.query({
      query: ({ id, type }) => ({
        url: `/movies/${id}`,
        params: { mediaType: type },
      }),
      transformResponse: (response) => {
        if (!response) return null;
        const item = response;
        return {
          id: item.id,
          tmdbId: item.tmdbId,
          mediaType: item.mediaType,
          title: item.title || item.name,
          posterPath: item.posterPath,
          backdropPath: item.backdropPath,
          voteAverage: item.voteAverage || 0,
          releaseDate: item.releaseDate || item.firstAirDate,
          overview: item.overview,
          isLiked: item.isLiked || false,
          isWatchlisted: item.isWatchlisted || false,
          runtime: item.runtime,
          genres: item.genres || [],
          status: item.status,
          tagline: item.tagline,
        };
      },
      providesTags: (result) =>
        result ? [{ type: 'Media', id: result.tmdbId }] : [],
    }),

    getMovieCredits: builder.query({
      query: ({ id, type }) => ({
        url: `/movies/${type}/${id}/credits`,
      }),
      transformResponse: (response) => {
        if (!response) return { cast: [], crew: [] };
        return {
          cast: response.cast || [],
          crew: response.crew || [],
        };
      },
    }),

    getMovieVideos: builder.query({
      query: ({ id, type }) => ({
        url: `/movies/${type}/${id}/videos`,
      }),
      transformResponse: (response) => {
        if (!response) return { results: [] };
        return {
          results: response.results || [],
        };
      },
    }),

    toggleLike: builder.mutation({
      query: ({ id, mediaType }) => ({
        url: `/userinteractions/movies/${id}/like`,
        method: 'POST',
        params: { mediaType },
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'Media', id },
        { type: 'MediaList' },
      ],
    }),

    getGenres: builder.query({
      query: ({ mediaType }) => ({
        url: '/genres',
        params: { mediaType },
      }),
      transformResponse: (response) => {
        if (!response) return [];

        return response.map((genre) => ({
          id: genre.tmdbId,
          name: genre.name,
        }));
      },
      providesTags: ['Genres'],
    }),

    getWatchlist: builder.query({
      query: ({ page = 1, pageSize = 20 }) => ({
        url: '/userinteractions/watchlist',
        params: { page, pageSize },
      }),
      transformResponse: (response) => {
        if (!response?.data)
          return { items: [], totalCount: 0, currentPage: 1, totalPages: 1 };
        return {
          items: response.data,
          totalCount: response.totalCount,
          currentPage: response.currentPage,
          totalPages: response.totalPages,
          hasNextPage: response.hasNextPage,
          hasPreviousPage: response.hasPreviousPage,
        };
      },
      providesTags: (result) =>
        result?.items
          ? [
              ...result.items.map(({ tmdbId }) => ({
                type: 'Media',
                id: tmdbId,
              })),
              { type: 'Watchlist' },
            ]
          : [{ type: 'Watchlist' }],
    }),

    getAllMedia: builder.query({
      query: ({
        page = 1,
        pageSize = 20,
        genres = [],
        search = '',
        mediaType = 'all',
        sortBy,
        year,
      } = {}) => {
        const params = new URLSearchParams({
          page: page.toString(),
          pageSize: pageSize.toString(),
          mediaType,
        });

        if (genres.length > 0) {
          params.append('withGenres', genres.join(','));
        }

        if (search) {
          params.append('search', search);
        }

        if (sortBy) {
          params.append('sortBy', sortBy);
        }

        if (year) {
          params.append('year', year);
        }

        return {
          url: '/movies',
          params,
        };
      },
      transformResponse: (response) => {
        if (!response?.data) return [];
        return response.data;
      },
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ tmdbId }) => ({
                type: 'Media',
                id: tmdbId,
              })),
              { type: 'MediaList' },
            ]
          : [{ type: 'MediaList' }],
    }),

    toggleWatchlist: builder.mutation({
      query: ({ id, mediaType, title }) => ({
        url: '/userinteractions/watchlist',
        method: 'POST',
        body: { id, mediaType, title },
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'Media', id },
        { type: 'MediaList' },
        { type: 'Watchlist' },
      ],
    }),
  }),
});

export const {
  useGetTrendingMoviesQuery,
  useGetTrendingShowsQuery,
  useSearchMediaQuery,
  useGetMovieDetailsQuery,
  useGetMovieCreditsQuery,
  useGetMovieVideosQuery,
  useToggleLikeMutation,
  useToggleWatchlistMutation,
  useGetGenresQuery,
  useGetWatchlistQuery,
  useGetAllMediaQuery,
} = mediaApi;
