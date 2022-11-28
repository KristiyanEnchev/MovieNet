import React from 'react';
import ErrorBoundary from './components/shared/ErrorBoundary';
import { Navbar } from './components/Navbar';
import { ThemeProvider } from './contexts/ThemeContext';

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider>
        <div className="min-h-screen flex flex-col bg-background text-foreground">
          <Navbar />
          <main className="flex-1 pt-18"> </main>
        </div>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
