import React from 'react';
import { MovieCard } from '../movie/MovieCard';

export function TrendingMovies({ movies = [] }) {
  if (!movies?.length) return null;

  return (
    <section className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold">Trending Movies</h2>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
        {movies.map((movie) => (
          <MovieCard
            key={movie.tmdbId || movie.id}
            movie={movie}
            type="movie"
          />
        ))}
      </div>
    </section>
  );
}
