import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { selectIsAuthenticated } from '../features/auth/authSlice';
import { useGetWatchlistQuery } from '../features/media/mediaApi';
import { MovieCard } from '../components/movie/MovieCard';
import PageLayout from '../components/shared/PageLayout';
import Loading from '../components/shared/Loading';

const PAGE_SIZE = 20;

const Watchlist = () => {
  const navigate = useNavigate();
  const isAuthenticated = useSelector(selectIsAuthenticated);
  const [page, setPage] = useState(1);

  const {
    data: watchlistData,
    isLoading,
    error,
  } = useGetWatchlistQuery(
    {
      page,
      pageSize: PAGE_SIZE,
    },
    {
      skip: !isAuthenticated,
      refetchOnMountOrArgChange: true
    }
  );

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  if (!isAuthenticated) {
    return null;
  }

  if (isLoading) {
    return (
      <PageLayout>
        <Loading />
      </PageLayout>
    );
  }

  if (error) {
    return (
      <PageLayout>
        <div className="container mx-auto px-4 py-20">
          <div className="text-center">
            <h2 className="text-xl font-semibold text-destructive">
              Error loading watchlist
            </h2>
            <p className="text-muted-foreground">{error.message}</p>
          </div>
        </div>
      </PageLayout>
    );
  }
  if (!watchlistData?.items?.length) {
    return (
      <PageLayout>
        <div className="container mx-auto px-4 py-20">
          <div className="text-center">
            <h2 className="text-xl font-semibold">Your watchlist is empty</h2>
            <p className="text-muted-foreground mt-2">
              Add movies and shows to your watchlist to keep track of what you
              want to watch!
            </p>
          </div>
        </div>
      </PageLayout>
    );
  }

  const totalPages = Math.ceil(watchlistData.totalCount / PAGE_SIZE);

  return (
    <PageLayout>
      <div className="container mx-auto px-4 py-20">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">Your Watchlist</h1>
          <div className="text-sm text-muted-foreground">
            {watchlistData.totalCount}{' '}
            {watchlistData.totalCount === 1 ? 'item' : 'items'}
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
          {watchlistData.items.map((media) => (
            <MovieCard
              key={media.tmdbId}
              movie={media}
              type={media.mediaType || 'movie'}
            />
          ))}
        </div>

        {totalPages > 1 && (
          <div className="flex justify-center gap-2 mt-8">
            <button
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 rounded-lg bg-primary/10 hover:bg-primary/20 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Previous
            </button>
            <span className="px-4 py-2">
              Page {page} of {totalPages}
            </span>
            <button
              onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-4 py-2 rounded-lg bg-primary/10 hover:bg-primary/20 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </PageLayout>
  );
};

export default Watchlist;
