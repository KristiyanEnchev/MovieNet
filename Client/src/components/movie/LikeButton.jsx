import React from 'react';
import { ThumbsUp } from 'lucide-react';
import { cn } from '../../lib/utils';
import { useUserInteractions } from '../../hooks/useUserInteractions';

const LikeButton = ({ 
  movie,
  variant = 'icon',
  className 
}) => {
  const {
    isLikeProcessing,
    interaction: { isLiked },
    actions: { toggleLike }
  } = useUserInteractions(movie);

  const handleClick = (e) => {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    toggleLike();
  };

  const isDisabled = isLikeProcessing;

  if (variant === 'full') {
    return (
      <button
        onClick={handleClick}
        disabled={isDisabled}
        className={cn(
          'flex items-center gap-2 px-4 py-2 rounded-lg transition-colors',
          isLiked 
            ? 'bg-primary text-primary-foreground hover:bg-primary/90'
            : 'bg-primary/10 hover:bg-primary/20',
          isDisabled && 'opacity-50 cursor-not-allowed',
          className
        )}
      >
        <ThumbsUp className={cn('w-5 h-5', isLiked && 'fill-current')} />
        <span>Like</span>
      </button>
    );
  }

  return (
    <button
      onClick={handleClick}
      disabled={isDisabled}
      className={cn(
        'p-2 rounded-lg transition-colors',
        isLiked
          ? 'bg-primary text-primary-foreground hover:bg-primary/90'
          : 'bg-primary/10 hover:bg-primary/20',
        isDisabled && 'opacity-50 cursor-not-allowed',
        className
      )}
    >
      <ThumbsUp className={cn('w-5 h-5', isLiked && 'fill-current')} />
    </button>
  );
};

export default LikeButton;