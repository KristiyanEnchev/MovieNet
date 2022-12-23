import React from 'react';
import {
  Building2,
  Globe,
  Clock,
  DollarSign,
  BarChart2,
  Calendar,
} from 'lucide-react';

export function MovieInfo({ movie }) {
  const formatCurrency = (value) => {
    if (!value) return 'N/A';
    return `$${value.toLocaleString()}`;
  };

  const formatRuntime = (minutes) => {
    if (!minutes) return 'N/A';
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return `${hours}h ${remainingMinutes}m`;
  };

  const formatDate = (dateString) => {
    if (!dateString || dateString === '0001-01-01T00:00:00') return 'N/A';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  return (
    <div className="p-6 rounded-lg bg-card overflow-x-auto">
      <div className="flex gap-8 min-w-max">
        <div className="flex items-center gap-3">
          <Building2 className="w-5 h-5 text-muted-foreground shrink-0" />
          <div>
            <p className="text-sm text-muted-foreground">Status</p>
            <p className="font-medium whitespace-nowrap">
              {movie.status || 'Unknown'}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <Globe className="w-5 h-5 text-muted-foreground shrink-0" />
          <div>
            <p className="text-sm text-muted-foreground">Language</p>
            <p className="font-medium whitespace-nowrap">
              {movie.originalLanguage?.toUpperCase() || 'Unknown'}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <Clock className="w-5 h-5 text-muted-foreground shrink-0" />
          <div>
            <p className="text-sm text-muted-foreground">Runtime</p>
            <p className="font-medium whitespace-nowrap">
              {formatRuntime(movie.runtime)}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <DollarSign className="w-5 h-5 text-muted-foreground shrink-0" />
          <div>
            <p className="text-sm text-muted-foreground">Budget</p>
            <p className="font-medium whitespace-nowrap">
              {formatCurrency(movie.budget)}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <BarChart2 className="w-5 h-5 text-muted-foreground shrink-0" />
          <div>
            <p className="text-sm text-muted-foreground">Revenue</p>
            <p className="font-medium whitespace-nowrap">
              {formatCurrency(movie.revenue)}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <Calendar className="w-5 h-5 text-muted-foreground shrink-0" />
          <div>
            <p className="text-sm text-muted-foreground">Release Date</p>
            <p className="font-medium whitespace-nowrap">
              {formatDate(movie.releaseDate)}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
