import React from 'react';
import { Star, Clock, Calendar } from 'lucide-react';
import { cn } from '../../lib/utils';
import WatchlistButton from './WatchlistButton';
import LikeButton from './LikeButton';

function RatingCircle({ rating = 0, size = 'lg' }) {
  const percentage = Math.round(((rating || 0) / 10) * 100);
  const isSmall = size === 'sm';
  const radius = isSmall ? 18 : 30;
  const circumference = 2 * Math.PI * radius;
  const strokeDashoffset = circumference - (percentage / 100) * circumference;

  const getColor = (percentage) => {
    if (percentage >= 70) return '#22c55e';
    if (percentage >= 50) return '#eab308';
    return '#ef4444';
  };

  const dimensions = {
    sm: {
      size: 'w-[48px] h-[48px]',
      center: { x: '20', y: '20' },
      strokeWidth: '3',
      fontSize: 'text-sm',
      symbolSize: 'text-[10px]',
    },
    lg: {
      size: 'w-[68px] h-[68px]',
      center: { x: '32', y: '32' },
      strokeWidth: '4',
      fontSize: 'text-lg',
      symbolSize: 'text-xs',
    },
  };

  const config = dimensions[size];

  return (
    <div
      className={cn(
        'relative bg-background rounded-full flex items-center justify-center border-2 border-background',
        config.size
      )}
    >
      <svg className="absolute w-full h-full -rotate-90">
        <circle
          cx={config.center.x}
          cy={config.center.y}
          r={radius}
          className="fill-none stroke-muted"
          strokeWidth={config.strokeWidth}
        />
        <circle
          cx={config.center.x}
          cy={config.center.y}
          r={radius}
          className="fill-none"
          strokeWidth={config.strokeWidth}
          strokeDasharray={circumference}
          strokeDashoffset={strokeDashoffset}
          style={{ stroke: getColor(percentage) }}
        />
      </svg>
      <div
        className={cn(
          'font-bold z-10 flex items-center justify-center translate-x-[2px] translate-y-[2px]',
          config.fontSize
        )}
      >
        <span className="inline-block leading-none">{percentage}</span>
        <span
          className={cn(
            'inline-block leading-none ml-[1px]',
            config.symbolSize
          )}
        >
          %
        </span>
      </div>
    </div>
  );
}

export const HeroBanner = React.memo(function HeroBanner({
  movie,
  type,
  isAuthenticated,
}) {
  const formatCount = (count = 0) => (count || 0).toLocaleString();
  const formatDuration = (minutes) => {
    if (!minutes) return 'N/A';
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return `${hours}h ${remainingMinutes}m`;
  };
  const formatReleaseDate = (date) => {
    if (!date) return 'TBA';
    return new Date(date).getFullYear();
  };
  const formatFullDate = (date) => {
    if (!date) return 'TBA';
    return new Date(date).toLocaleDateString();
  };

  return (
    <div className="relative min-h-[90vh] md:min-h-[80vh] w-full pt-16">
      {/* Background Image */}
      <div
        className="absolute inset-0 bg-cover bg-center"
        style={{ backgroundImage: `url(${movie.backdropPath || ''})` }}
      >
        <div className="absolute inset-0 bg-gradient-to-t from-background via-background/90 to-transparent" />
      </div>

      {/* Content */}
      <div className="relative h-full flex flex-col">
        <div className="container mx-auto px-4 py-4 md:py-8 flex-1 flex flex-col">
          {/* Mobile Layout */}
          <div className="block md:hidden mb-4">
            <h1 className="text-2xl sm:text-3xl font-bold mb-2">
              {movie.title}
            </h1>
            {movie.releaseDate && (
              <div className="text-lg sm:text-xl text-muted-foreground">
                ({formatReleaseDate(movie.releaseDate)})
              </div>
            )}
          </div>

          <div className="flex-1 flex flex-col md:grid md:grid-cols-[250px_1fr] gap-6 md:gap-8">
            {/* Poster */}
            <div className="relative w-40 sm:w-48 md:w-full mx-auto shrink-0">
              <img
                src={movie.posterPath || ''}
                alt={movie.title}
                className="w-full rounded-lg shadow-xl"
              />
            </div>

            {/* Details */}
            <div className="flex flex-col">
              {/* Desktop Title */}
              <div className="hidden md:flex items-center gap-4 mb-4">
                <h1 className="text-4xl font-bold">{movie.title}</h1>
                {movie.releaseDate && (
                  <span className="text-2xl text-muted-foreground">
                    ({formatReleaseDate(movie.releaseDate)})
                  </span>
                )}
              </div>

              {/* Rating and Stats */}
              <div className="flex flex-wrap items-center gap-4 md:gap-6 mb-6">
                <div className="flex items-center gap-4 md:gap-6">
                  <RatingCircle
                    rating={movie.voteAverage}
                    size={window.innerWidth < 768 ? 'sm' : 'lg'}
                  />
                  <div className="flex items-center gap-2">
                    <Star className="w-5 h-5 md:w-6 md:h-6 text-yellow-400 fill-yellow-400" />
                    <span className="text-lg md:text-xl font-semibold">
                      {(movie.voteAverage || 0).toFixed(1)}
                    </span>
                    <span className="text-xs md:text-sm text-muted-foreground">
                      ({formatCount(movie.voteCount)} ratings)
                    </span>
                  </div>
                </div>

                {/* Stats content */}
                <div className="flex flex-wrap gap-4">
                  {movie.runtime > 0 && (
                    <div className="flex items-center gap-2 text-muted-foreground">
                      <Clock className="w-4 h-4 md:w-5 md:h-5" />
                      <span>{formatDuration(movie.runtime)}</span>
                    </div>
                  )}

                  {movie.releaseDate && (
                    <div className="flex items-center gap-2 text-muted-foreground">
                      <Calendar className="w-4 h-4 md:w-5 md:h-5" />
                      <span>{formatFullDate(movie.releaseDate)}</span>
                    </div>
                  )}
                </div>
              </div>

              {/* Tagline */}
              {movie.tagline && (
                <div className="p-2 rounded-lg bg-card mb-4">
                  <p className="text-base md:text-lg italic text-muted-foreground">
                    "{movie.tagline}"
                  </p>
                </div>
              )}

              {/* Genres */}
              <div className="flex flex-wrap gap-2 mb-4">
                {movie.genres?.map((genre, index) => (
                  <span
                    key={`${genre.id}-${index}`}
                    className="px-2 py-1 md:px-3 md:py-1 rounded-full bg-primary/10 text-primary text-xs md:text-sm"
                  >
                    {genre.name}
                  </span>
                ))}
              </div>

              {/* Overview */}
              <p className="text-sm sm:text-base md:text-lg mb-6 max-w-3xl">
                {movie.overview}
              </p>

              {/* Actions */}
              <div className="flex gap-4 mt-auto">
                <WatchlistButton movie={movie} variant="full" />
                {isAuthenticated && (
                  <LikeButton movie={movie} variant="full" />
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
});
