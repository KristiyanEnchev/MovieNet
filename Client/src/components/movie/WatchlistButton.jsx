import React from 'react';
import { Bookmark, BookmarkCheck } from 'lucide-react';
import { cn } from '../../lib/utils';
import { useUserInteractions } from '../../hooks/useUserInteractions';

const WatchlistButton = ({ 
  movie,
  variant = 'icon',
  className 
}) => {
  const {
    isWatchlistProcessing,
    interaction: { isWatchlisted },
    actions: { toggleWatchlist }
  } = useUserInteractions(movie);

  const handleClick = (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    toggleWatchlist();
  };

  const isDisabled = isWatchlistProcessing;

  if (variant === 'full') {
    return (
      <button
        onClick={handleClick}
        disabled={isDisabled}
        className={cn(
          'flex items-center gap-2 px-4 py-2 rounded-lg transition-colors',
          isWatchlisted 
            ? 'bg-primary text-primary-foreground hover:bg-primary/90'
            : 'bg-primary/10 hover:bg-primary/20',
          isDisabled && 'opacity-50 cursor-not-allowed',
          className
        )}
      >
        {isWatchlisted ? (
          <BookmarkCheck className="w-5 h-5 fill-current" />
        ) : (
          <Bookmark className="w-5 h-5" />
        )}
        <span>
          {isDisabled 
            ? 'Updating...' 
            : isWatchlisted 
              ? 'Remove from Watchlist'
              : 'Add to Watchlist'}
        </span>
      </button>
    );
  }

  return (
    <button
      onClick={handleClick}
      disabled={isDisabled}
      className={cn(
        'p-2 rounded-lg transition-colors',
        isWatchlisted
          ? 'bg-primary text-primary-foreground hover:bg-primary/90'
          : 'bg-primary/10 hover:bg-primary/20',
        isDisabled && 'opacity-50 cursor-not-allowed',
        className
      )}
    >
      {isWatchlisted ? (
        <BookmarkCheck className="w-5 h-5 fill-current" />
      ) : (
        <Bookmark className="w-5 h-5" />
      )}
    </button>
  );
};

export default WatchlistButton;
