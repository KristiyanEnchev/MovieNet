import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { ThemeProvider } from './contexts/ThemeContext';
import { Navbar } from './components/Navbar';
import { AuthRoute } from './components/auth/ProtectedRoute';
import { Footer } from './components/shared/Footer';
import ErrorBoundary from './components/shared/ErrorBoundary';

// Pages
import LoginPage from './pages/auth/LoginPage';

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider>
        <div className="min-h-screen flex flex-col bg-background text-foreground">
          <Navbar />
          <main className="flex-1 pt-18">
            {' '}
            <Routes>
              {' '}
              <Route
                path="/login"
                element={
                  <AuthRoute>
                    <LoginPage />
                  </AuthRoute>
                }
              />
            </Routes>
          </main>
          <Footer />
        </div>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
