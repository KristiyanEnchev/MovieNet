import React from 'react';
import { useSelector } from 'react-redux';
import { Star } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

import { selectIsAuthenticated } from '../../features/auth/authSlice';
import WatchlistButton from './WatchlistButton';
import LikeButton from './LikeButton';

export function MovieCard({ movie, type = 'movie' }) {
  const navigate = useNavigate();
  const isAuthenticated = useSelector(selectIsAuthenticated);

  const displayTitle = movie.title || movie.name;
  const rating = movie.voteAverage
    ? Number(movie.voteAverage).toFixed(1)
    : 'N/A';
  const year =
    movie.releaseDate || movie.firstAirDate
      ? new Date(movie.releaseDate || movie.firstAirDate).getFullYear()
      : 'N/A';

  return (
    <div
      onClick={() =>
        navigate(
          `/${type === 'movie' ? 'movies' : 'shows'}/${movie.tmdbId}/${type}`
        )
      }
      className="group relative flex flex-col rounded-lg overflow-hidden bg-card hover:scale-105 transition-transform duration-200 cursor-pointer"
    >
      <div className="relative aspect-[2/3] w-full overflow-hidden rounded-lg">
        <img
          src={movie.posterPath || '/no-poster.png'}
          alt={displayTitle}
          className="object-cover w-full h-full transition-opacity group-hover:opacity-80"
          loading="lazy"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent opacity-0 transition-opacity duration-300 group-hover:opacity-100" />
        <div className="absolute bottom-0 right-0 flex gap-2 p-2">
          <WatchlistButton movie={movie} />
          {isAuthenticated && <LikeButton movie={movie} />}
        </div>
      </div>

      <div className="p-4">
        <h3 className="font-medium leading-tight line-clamp-2">
          {displayTitle}
        </h3>
        <div className="mt-1 flex items-center gap-2 text-sm text-muted-foreground">
          <div className="flex items-center gap-1">
            <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
            <span>{rating}</span>
          </div>
          <span>•</span>
          <span>{year}</span>
        </div>
      </div>
    </div>
  );
}
