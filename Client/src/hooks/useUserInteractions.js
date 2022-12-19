import { useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import {
  useToggleLikeMutation,
  useToggleWatchlistMutation,
} from '../features/media/mediaApi.js';
import { selectIsAuthenticated } from '../features/auth/authSlice.js';
import { useDebounceAction } from './useDebounceAction.js';

export const useUserInteractions = (movie, options = {}) => {
  const {
    skip = false,
    initialData = { isLiked: false, isWatchlisted: false },
  } = options;

  const navigate = useNavigate();
  const isAuthenticated = useSelector(selectIsAuthenticated);
  const [isLikeProcessing, setIsLikeProcessing] = useState(false);
  const [isWatchlistProcessing, setIsWatchlistProcessing] = useState(false);

  const shouldSkip = skip || !isAuthenticated || !movie?.tmdbId || !movie?.mediaType;

  const [toggleLike] = useToggleLikeMutation();
  const [toggleWatchlist] = useToggleWatchlistMutation();

  const handleToggleLike = useCallback(async () => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    if (!movie?.tmdbId || !movie?.mediaType) {
      return;
    }

    try {
      setIsLikeProcessing(true);
      await toggleLike({
        id: movie.tmdbId,
        mediaType: movie.mediaType,
      }).unwrap();
    } catch (error) {
      console.error('Failed to toggle like:', error);
    } finally {
      setIsLikeProcessing(false);
    }
  }, [movie?.tmdbId, movie?.mediaType, isAuthenticated, navigate, toggleLike]);

  const handleToggleWatchlist = useCallback(
    async () => {
      if (!isAuthenticated) {
        navigate('/login');
        return;
      }

      if (isWatchlistProcessing || !movie?.tmdbId || !movie?.mediaType || !movie?.title) {
        return;
      }

      setIsWatchlistProcessing(true);
      try {
        await toggleWatchlist({
          id: movie.tmdbId,
          mediaType: movie.mediaType,
          title: movie.title,
        }).unwrap();
      } catch (error) {
        console.error('Failed to toggle watchlist:', error);
      } finally {
        setIsWatchlistProcessing(false);
      }
    },
    [
      movie?.tmdbId,
      movie?.mediaType,
      movie?.title,
      isAuthenticated,
      navigate,
      toggleWatchlist,
      isWatchlistProcessing,
    ]
  );

  const debouncedToggleLike = useDebounceAction(handleToggleLike, 300);
  const debouncedToggleWatchlist = useDebounceAction(
    handleToggleWatchlist,
    300
  );

  return {
    isAuthenticated,
    isLikeProcessing,
    isWatchlistProcessing,
    interaction: shouldSkip ? initialData : {
      isLiked: movie?.isLiked || false,
      isWatchlisted: movie?.isWatchlisted || false,
      rating: movie?.rating || 0,
    },
    actions: {
      toggleLike: debouncedToggleLike,
      toggleWatchlist: debouncedToggleWatchlist,
    },
  };
};
