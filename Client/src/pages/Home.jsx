import React, { useState, useCallback, useEffect } from 'react';
import {
  useGetTrendingMoviesQuery,
  useGetTrendingShowsQuery,
} from '../features/media/mediaApi';
import Loading from '../components/shared/Loading';
import PageLayout from '../components/shared/PageLayout';
import { HeroCarousel } from '../components/home/HeroCarousel';
import { TrendingMovies } from '../components/home/TrendingMovies';
import { TrendingShows } from '../components/home/TrendingShows';
import { MovieCard } from '../components/movie/MovieCard';

export default function Home() {
  const {
    data: movies = [],
    isLoading: isLoadingMovies,
    isError: isMoviesError,
    refetch: refetchMovies,
  } = useGetTrendingMoviesQuery({});

  const {
    data: shows = [],
    isLoading: isLoadingShows,
    isError: isShowsError,
    refetch: refetchShows,
  } = useGetTrendingShowsQuery({});

  const [searchState, setSearchState] = useState({
    results: null,
    isSearching: false,
    searchQuery: '',
  });

  const handleSearchResults = useCallback((newState) => {
    setSearchState(newState);
  }, []);

  const isFetching = isLoadingMovies || isLoadingShows;
  const isError = isMoviesError || isShowsError;

  const handleRetry = useCallback(() => {
    refetchMovies();
    refetchShows();
  }, [refetchMovies, refetchShows]);

  const [loadingTimeout, setLoadingTimeout] = useState(false);

  useEffect(() => {
    let timer;
    if (isFetching) {
      timer = setTimeout(() => {
        setLoadingTimeout(true);
      }, 10000);
    }
    return () => clearTimeout(timer);
  }, [isFetching]);

  if (isFetching && !loadingTimeout) {
    return (
      <PageLayout>
        <div className="flex items-center justify-center min-h-[50vh]">
          <Loading />
        </div>
      </PageLayout>
    );
  }

  if (loadingTimeout || isError) {
    return (
      <PageLayout
        showSearch
        onSearchResults={handleSearchResults}
        containerClassName="py-20"
      >
        <div className="text-center py-20">
          <h2 className="text-2xl font-bold text-destructive mb-4">
            {loadingTimeout
              ? 'Taking Too Long to Load'
              : 'Unable to Load Content'}
          </h2>
          <p className="text-muted-foreground mb-4">
            {loadingTimeout
              ? 'The connection seems to be slow. Please try again.'
              : 'There was a problem loading the content.'}
          </p>
          <button
            onClick={handleRetry}
            className="px-4 py-2 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
          >
            Retry Loading
          </button>
        </div>
      </PageLayout>
    );
  }

  if (searchState.searchQuery) {
    return (
      <PageLayout
        showSearch
        onSearchResults={handleSearchResults}
        containerClassName="py-20"
      >
        <h2 className="text-2xl font-bold mb-6">Search Results</h2>
        {searchState.isSearching ? (
          <Loading />
        ) : searchState.results?.length > 0 ? (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
            {searchState.results.map((item) => (
              <MovieCard key={item.tmdbId} movie={item} type={item.mediaType} />
            ))}
          </div>
        ) : (
          <p className="text-center text-lg text-muted-foreground">
            No results found for "{searchState.searchQuery}"
          </p>
        )}
      </PageLayout>
    );
  }

  const heroContent = movies?.length > 0 && (
    <div className="pt-4">
      <HeroCarousel items={movies} />
    </div>
  );

  return (
    <PageLayout
      heroContent={heroContent}
      showSearch
      onSearchResults={handleSearchResults}
      containerClassName="py-20"
    >
      {!movies?.length && !shows?.length ? (
        <div className="text-center py-20">
          <h2 className="text-2xl font-bold mb-4">No Content Available</h2>
          <p className="text-muted-foreground">
            There are currently no trending movies or shows to display.
          </p>
        </div>
      ) : (
        <>
          {movies?.length > 0 && (
            <TrendingMovies movies={movies.slice(0, 10)} />
          )}
          {shows?.length > 0 && <TrendingShows shows={shows.slice(0, 10)} />}
        </>
      )}
    </PageLayout>
  );
}
