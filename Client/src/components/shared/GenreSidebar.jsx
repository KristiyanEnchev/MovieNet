import React from 'react';
import { useGetGenresQuery } from '../../features/media/mediaApi';
import { cn } from '../../lib/utils';

export function GenreSidebar({ mediaType, selectedGenres = [], onGenreClick }) {
  const { data: genres, isLoading, error } = useGetGenresQuery({ mediaType });

  if (isLoading) {
    return (
      <aside className="py-20 w-48 p-4 border-r border-border">
        <h2 className="text-lg font-semibold mb-4">Genres</h2>
        <div className="space-y-2">
          {[...Array(8)].map((_, i) => (
            <div
              key={i}
              className="h-8 bg-muted rounded-lg animate-pulse"
            />
          ))}
        </div>
      </aside>
    );
  }

  if (error) {
    console.error('Genre fetch error:', error);
    return (
      <aside className="py-20 w-48 p-4 border-r border-border">
        <h2 className="text-lg font-semibold mb-4">Genres</h2>
        <p className="text-sm text-muted-foreground">Failed to load genres</p>
      </aside>
    );
  }

  if (!genres || genres.length === 0) {
    return (
      <aside className="py-20 w-48 p-4 border-r border-border">
        <h2 className="text-lg font-semibold mb-4">Genres</h2>
        <p className="text-sm text-muted-foreground">No genres available</p>
      </aside>
    );
  }

  return (
    <aside className="py-20 w-48 p-4 border-r border-border">
      <h2 className="text-lg font-semibold mb-4">Genres</h2>
      <div className="space-y-2">
        {genres.map((genre) => (
          <button
            key={genre.id}
            onClick={() => onGenreClick(genre.id)}
            className={cn(
              "w-full px-3 py-2 text-left rounded-lg transition-colors",
              selectedGenres.includes(genre.id)
                ? "bg-primary text-primary-foreground hover:bg-primary/90"
                : "hover:bg-muted"
            )}
          >
            {genre.name}
          </button>
        ))}
      </div>
    </aside>
  );
}
