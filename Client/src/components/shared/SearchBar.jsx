import React, { useState, useCallback, useEffect } from 'react';
import { Search as SearchIcon } from 'lucide-react';
import { useSearchMediaQuery } from '../../features/media/mediaApi';
import { cn } from '../../lib/utils';
import { useDebounce } from '../../hooks/useDebounce';

export default function SearchBar({
  mediaType = 'multi',
  onSearchResults,
  className,
  initialValue = '',
  placeholder = 'Search for movies, TV shows...',
  minSearchLength = 2,
}) {
  const [searchTerm, setSearchTerm] = useState(initialValue);
  const [activeSearch, setActiveSearch] = useState(initialValue);

  const debouncedSearchTerm = useDebounce(searchTerm, 500);

  const {
    data: searchResults,
    isFetching,
    isError,
  } = useSearchMediaQuery(
    { query: activeSearch, mediaType },
    {
      skip: !activeSearch?.trim() || activeSearch.length < minSearchLength,
      refetchOnMountOrArgChange: false,
    }
  );

  useEffect(() => {
    if (onSearchResults) {
      onSearchResults({
        results: searchResults || [],
        isSearching: isFetching,
        searchQuery: activeSearch || '',
      });
    }
  }, [searchResults, isFetching, activeSearch, onSearchResults]);

  useEffect(() => {
    const trimmedTerm = debouncedSearchTerm.trim();
    if (trimmedTerm && trimmedTerm.length >= minSearchLength) {
      setActiveSearch(trimmedTerm);
    } else if (!trimmedTerm) {
      setActiveSearch('');
    }
  }, [debouncedSearchTerm, minSearchLength]);

  const handleSubmit = useCallback(
    (e) => {
      e.preventDefault();
      const trimmedTerm = searchTerm.trim();
      if (trimmedTerm && trimmedTerm.length >= minSearchLength) {
        setActiveSearch(trimmedTerm);
      }
    },
    [searchTerm, minSearchLength]
  );

  const handleChange = useCallback((e) => {
    const newTerm = e.target.value;
    setSearchTerm(newTerm);
  }, []);

  const handleClear = useCallback(() => {
    setSearchTerm('');
    setActiveSearch('');
  }, []);

  return (
    <form
      onSubmit={handleSubmit}
      className={cn('relative', className)}
      role="search"
    >
      <div className="relative">
        <SearchIcon
          className="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-muted-foreground"
          aria-hidden="true"
        />
        <input
          type="search"
          value={searchTerm}
          onChange={handleChange}
          placeholder={placeholder}
          className="w-full pl-12 pr-16 py-3 rounded-lg bg-accent text-foreground
                   placeholder:text-muted-foreground focus:outline-none
                   focus:ring-2 focus:ring-primary"
          aria-label="Search input"
          autoComplete="off"
          minLength={minSearchLength}
        />
        {searchTerm && (
          <button
            type="button"
            onClick={handleClear}
            className="absolute right-12 top-1/2 -translate-y-1/2 p-2 rounded-md
                     hover:bg-primary/10 transition-colors"
            aria-label="Clear search"
          >
            <span className="sr-only">Clear search</span>×
          </button>
        )}
        <button
          type="submit"
          className={cn(
            'absolute right-2 top-1/2 -translate-y-1/2 p-2 rounded-md',
            'hover:bg-primary/10 transition-colors',
            'disabled:opacity-50 disabled:cursor-not-allowed'
          )}
          aria-label="Submit search"
          disabled={
            isFetching ||
            !searchTerm.trim() ||
            searchTerm.length < minSearchLength
          }
        >
          <SearchIcon
            className={cn(
              'h-5 w-5',
              isFetching ? 'text-muted-foreground' : 'text-primary'
            )}
          />
        </button>
      </div>
      {isError && (
        <p className="mt-2 text-sm text-destructive" role="alert">
          An error occurred while searching. Please try again.
        </p>
      )}
    </form>
  );
}
