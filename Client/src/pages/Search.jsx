import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import { MovieCard } from '../components/movie/MovieCard';
import SearchBar from '../components/shared/SearchBar';
import PageLayout from '../components/shared/PageLayout';

export default function Search() {
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const initialQuery = searchParams.get('q') || '';
  const initialType = searchParams.get('type') || 'multi';

  const [searchState, setSearchState] = useState({
    results: [],
    isSearching: false,
    searchQuery: initialQuery,
  });

  const handleSearchResults = (newState) => {
    setSearchState(newState);
    const params = new URLSearchParams();
    if (newState.searchQuery?.trim()) {
      params.set('q', newState.searchQuery.trim());
      params.set('type', initialType);
      window.history.replaceState(
        null,
        '',
        `${location.pathname}?${params.toString()}`
      );
    } else {
      window.history.replaceState(null, '', location.pathname);
    }
  };

  return (
    <PageLayout containerClassName="py-20">
      <div className="max-w-2xl mx-auto">
        <SearchBar
          mediaType={initialType}
          onSearchResults={handleSearchResults}
          className="w-full"
          initialValue={initialQuery}
        />
      </div>

      {searchState.searchQuery && (
        <>
          <h2 className="text-2xl font-bold mb-6">
            Search Results for "{searchState.searchQuery}"
          </h2>
          {searchState.isSearching ? (
            <div className="flex items-center justify-center min-h-[50vh]">
              <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-primary" />
            </div>
          ) : searchState.results?.length > 0 ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
              {searchState.results.map((item) => (
                <MovieCard
                  key={item.tmdbId}
                  movie={item}
                  type={item.mediaType}
                />
              ))}
            </div>
          ) : (
            <div className="text-center py-20">
              <p className="text-lg text-muted-foreground">
                No results found for "{searchState.searchQuery}"
              </p>
            </div>
          )}
        </>
      )}
    </PageLayout>
  );
}
