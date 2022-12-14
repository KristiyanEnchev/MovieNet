import React from 'react';
import { MovieCard } from '../movie/MovieCard';

export function TrendingShows({ shows = [] }) {
  if (!shows?.length) return null;

  return (
    <section className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold">Trending TV Shows</h2>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
        {shows.map((show) => (
          <MovieCard
            key={show.tmdbId}
            movie={{
              ...show,
              title: show.title,
              posterUrl: show.poster_path,
            }}
            type="tv"
          />
        ))}
      </div>
    </section>
  );
}
