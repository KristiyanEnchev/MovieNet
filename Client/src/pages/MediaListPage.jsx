import React, { useState, useCallback, useEffect } from 'react';
import { useGetAllMediaQuery } from '../features/media/mediaApi';
import { MovieCard } from '../components/movie/MovieCard';
import { GenreSidebar } from '../components/shared/GenreSidebar';
import SearchBar from '../components/shared/SearchBar';
import PageLayout from '../components/shared/PageLayout';
import Loading from '../components/shared/Loading';
import { Pagination } from '../components/shared/Pagination';

export default function MediaListPage({ mediaType }) {
  const [selectedGenres, setSelectedGenres] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [loadingTimeout, setLoadingTimeout] = useState(false);
  const [searchState, setSearchState] = useState({
    results: null,
    isSearching: false,
    searchQuery: '',
  });

  const { data, isLoading, isError, refetch, isFetching } = useGetAllMediaQuery(
    {
      mediaType,
      page: currentPage,
      genres: selectedGenres,
    },
    {
      refetchOnMountOrArgChange: true
    }
  );

  const isInitialLoading = isLoading && !isFetching;
  const shouldShowLoading =
    isInitialLoading && !selectedGenres.length && !loadingTimeout;

  useEffect(() => {
    let timer;
    if (isInitialLoading) {
      setLoadingTimeout(false);
      timer = setTimeout(() => {
        setLoadingTimeout(true);
      }, 10000);
    }
    return () => clearTimeout(timer);
  }, [isInitialLoading]);

  const handleRetry = useCallback(() => {
    setLoadingTimeout(false);
    refetch();
  }, [refetch]);

  useEffect(() => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }, [currentPage]);

  const handleSearchResults = useCallback((newState) => {
    setSearchState(newState);
    setCurrentPage(1);
  }, []);

  const handleGenreClick = useCallback((genreId) => {
    setSelectedGenres((prev) => {
      const isSelected = prev.includes(genreId);
      return isSelected
        ? prev.filter((id) => id !== genreId)
        : [...prev, genreId];
    });
    setCurrentPage(1);
    setSearchState({
      results: null,
      isSearching: false,
      searchQuery: '',
    });
  }, []);

  const handlePageChange = useCallback((newPage) => {
    setCurrentPage(newPage);
  }, []);

  if (shouldShowLoading) {
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
      <PageLayout>
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

  const contentToDisplay = searchState.searchQuery ? searchState.results : data;
  const hasContent =
    Array.isArray(contentToDisplay) && contentToDisplay.length > 0;

  return (
    <PageLayout>
      <div className="flex">
        <GenreSidebar
          mediaType={mediaType}
          selectedGenres={selectedGenres}
          onGenreClick={handleGenreClick}
        />

        <div className="flex-1 px-4 py-20">
          <div className="max-w-2xl mx-auto mb-8">
            <SearchBar
              mediaType={mediaType}
              onSearchResults={handleSearchResults}
              className="w-full"
            />
          </div>

          <div className="flex justify-between items-center mb-6">
            <h2 className="text-2xl font-bold">
              {searchState.searchQuery
                ? 'Search Results'
                : selectedGenres.length > 0
                ? `Filtered ${mediaType === 'movie' ? 'Movies' : 'TV Shows'}`
                : `Popular ${mediaType === 'movie' ? 'Movies' : 'TV Shows'}`}
            </h2>
            {!searchState.searchQuery && hasContent && (
              <Pagination
                currentPage={currentPage}
                onPageChange={handlePageChange}
              />
            )}
          </div>

          {hasContent ? (
            <>
              <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4">
                {contentToDisplay.map((item) => (
                  <MovieCard key={item.tmdbId} movie={item} type={mediaType} />
                ))}
              </div>

              {!searchState.searchQuery && (
                <Pagination
                  currentPage={currentPage}
                  onPageChange={handlePageChange}
                />
              )}
            </>
          ) : (
            <p className="text-center text-lg text-muted-foreground">
              No {mediaType === 'movie' ? 'movies' : 'TV shows'} found.
            </p>
          )}
        </div>
      </div>
    </PageLayout>
  );
}
