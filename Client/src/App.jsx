import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { ThemeProvider } from './contexts/ThemeContext';
import { Navbar } from './components/Navbar';
import { ProtectedRoute } from './components/auth/ProtectedRoute';
import { AuthRoute } from './components/auth/ProtectedRoute';
import { SearchProvider } from './contexts/SearchContext';
import { Footer } from './components/shared/Footer';
import ErrorBoundary from './components/shared/ErrorBoundary';
import PageNotFound from './pages/PageNotFound';

// Pages
import Home from './pages/Home';
import MovieList from './pages/Movies';
import Shows from './pages/Shows';
import MovieDetails from './pages/MovieDetails';
import Search from './pages/Search';
import LoginPage from './pages/auth/LoginPage';
import SignupPage from './pages/auth/SignupPage';
import Watchlist from './pages/Watchlist';

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider>
        <SearchProvider>
          <div className="min-h-screen flex flex-col bg-background text-foreground">
            <Navbar />
            <main className="flex-1 pt-18">
              <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/movies" element={<MovieList />} />
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
            </main>
            <Footer />
          </div>
        </SearchProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
