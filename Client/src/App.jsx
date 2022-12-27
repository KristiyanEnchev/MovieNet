import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { ThemeProvider } from './contexts/ThemeContext';
import { AuthRoute } from './components/auth/ProtectedRoute';
import ErrorBoundary from './components/shared/ErrorBoundary';
import { ProtectedRoute } from './components/auth/ProtectedRoute';
import { Toaster } from 'react-hot-toast';

// Pages
import LoginPage from './pages/auth/LoginPage';
import SignupPage from './pages/auth/SignupPage';
import PageNotFound from './pages/PageNotFound';
import Home from './pages/Home';
import { SearchProvider } from './contexts/SearchContext';
import MovieDetails from './pages/MovieDetails';
import Search from './pages/Search';
import Shows from './pages/Shows';
import Movies from './pages/Movies';
import Watchlist from './pages/Watchlist';

function App() {
  return (
    <>
      <Toaster position="top-center" />
      <ErrorBoundary>
        <ThemeProvider>
          <SearchProvider>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/movies" element={<Movies />} />
              <Route path="/shows" element={<Shows />} />
              <Route
                path="/movies/:id/:type"
                element={<MovieDetails type="movie" />}
              />
              <Route
                path="/shows/:id/:type"
                element={<MovieDetails type="tv" />}
              />
              <Route path="/search" element={<Search />} />
              <Route
                path="/watchlist"
                element={
                  <ProtectedRoute>
                    <Watchlist />
                  </ProtectedRoute>
                }
              />
              <Route
                path="/login"
                element={
                  <AuthRoute>
                    <LoginPage />
                  </AuthRoute>
                }
              />
              <Route
                path="/signup"
                element={
                  <AuthRoute>
                    <SignupPage />
                  </AuthRoute>
                }
              />
              <Route path="*" element={<PageNotFound />} />
            </Routes>
          </SearchProvider>
        </ThemeProvider>
      </ErrorBoundary>
    </>
  );
}

export default App;
