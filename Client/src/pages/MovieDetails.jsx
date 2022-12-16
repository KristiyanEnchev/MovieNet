import React, { useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { skipToken } from '@reduxjs/toolkit/query';
import {
  useGetMovieDetailsQuery,
  useGetMovieCreditsQuery,
  useGetMovieVideosQuery,
} from '../features/media/mediaApi';
import { selectIsAuthenticated } from '../features/auth/authSlice';
import PageLayout from '../components/shared/PageLayout';
import { MovieInfo } from '../components/movie/MovieInfo';
import Loading from '../components/shared/Loading';
import ReviewsSection from '../components/movie/review/ReviewsSection';
import VideosSection from '../components/movie/VideosSection';
import CastSection from '../components/movie/CastSection';
import { HeroBanner } from '../components/movie/HeroBanner';

export default function MovieDetails() {
  const { id, type } = useParams();
  const isAuthenticated = useSelector(selectIsAuthenticated);
  const queryParams = id && type ? { id, type } : skipToken;

  const {
    data: movie,
    isLoading: isLoadingMovie,
    isSuccess: isMovieSuccess,
  } = useGetMovieDetailsQuery(queryParams);

  const { data: credits } = useGetMovieCreditsQuery(
    isMovieSuccess ? queryParams : skipToken,
    { skip: !isMovieSuccess }
  );

  const { data: videos } = useGetMovieVideosQuery(
    isMovieSuccess ? queryParams : skipToken,
    { skip: !isMovieSuccess }
  );

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [id]);

  if (isLoadingMovie) {
    return (
      <PageLayout>
        <Loading />
      </PageLayout>
    );
  }

  if (!movie) {
    return (
      <PageLayout>
        <div className="container mx-auto px-4 py-20">
          <h1 className="text-2xl font-bold text-center">Movie not found</h1>
        </div>
      </PageLayout>
    );
  }

  return (
    <PageLayout className="p-0">
      <HeroBanner
        movie={movie}
        id={id}
        type={type}
        isAuthenticated={isAuthenticated}
      />

      <div className="container mx-auto px-4">
        {/* Movie Info */}
        <div className="mb-10">
          <MovieInfo movie={movie} />
        </div>

        {/* Cast Section */}
        <CastSection cast={credits?.cast} />

        {/* Videos Section */}
        <VideosSection videos={videos?.results} />

        {/* Reviews Section */}
        <ReviewsSection reviews={movie.reviews} />
      </div>
    </PageLayout>
  );
}
