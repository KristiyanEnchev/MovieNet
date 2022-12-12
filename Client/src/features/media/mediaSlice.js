import { createSlice } from '@reduxjs/toolkit';
import { mediaApi } from './mediaApi';

const initialState = {
  trendingMovies: [],
  trendingShows: [],
  searchResults: [],
  loading: false,
  error: null,
};

const mediaSlice = createSlice({
  name: 'media',
  initialState,
  reducers: {
    clearSearchResults: (state) => {
      state.searchResults = [];
    },
  },
  extraReducers: (builder) => {
    builder
      // Trending Movies
      .addMatcher(
        mediaApi.endpoints.getTrendingMovies.matchPending,
        (state) => {
          state.loading = true;
        }
      )
      .addMatcher(
        mediaApi.endpoints.getTrendingMovies.matchFulfilled,
        (state, action) => {
          state.loading = false;
          state.trendingMovies = action.payload.results;
        }
      )
      .addMatcher(
        mediaApi.endpoints.getTrendingMovies.matchRejected,
        (state, action) => {
          state.loading = false;
          state.error = action.error.message;
        }
      )
      // Trending Shows
      .addMatcher(
        mediaApi.endpoints.getTrendingShows.matchPending,
        (state) => {
          state.loading = true;
        }
      )
      .addMatcher(
        mediaApi.endpoints.getTrendingShows.matchFulfilled,
        (state, action) => {
          state.loading = false;
          state.trendingShows = action.payload.results;
        }
      )
      .addMatcher(
        mediaApi.endpoints.getTrendingShows.matchRejected,
        (state, action) => {
          state.loading = false;
          state.error = action.error.message;
        }
      )
      // Search Media
      .addMatcher(
        mediaApi.endpoints.searchMedia.matchPending,
        (state) => {
          state.loading = true;
        }
      )
      .addMatcher(
        mediaApi.endpoints.searchMedia.matchFulfilled,
        (state, action) => {
          state.loading = false;
          state.searchResults = action.payload.results;
        }
      )
      .addMatcher(
        mediaApi.endpoints.searchMedia.matchRejected,
        (state, action) => {
          state.loading = false;
          state.error = action.error.message;
        }
      );
  },
});

// Selectors
export const selectTrendingMovies = (state) => state.media.trendingMovies;
export const selectTrendingShows = (state) => state.media.trendingShows;
export const selectSearchResults = (state) => state.media.searchResults;
export const selectLoading = (state) => state.media.loading;
export const selectError = (state) => state.media.error;

export const { clearSearchResults } = mediaSlice.actions;
export default mediaSlice.reducer;