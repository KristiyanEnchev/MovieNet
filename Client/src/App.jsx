import React from 'react';
import ErrorBoundary from './components/shared/ErrorBoundary';
import { Navbar } from './components/Navbar';

function App() {
  return (
    <ErrorBoundary>
      <div className="min-h-screen flex flex-col bg-background text-foreground">
        <Navbar />
        <main className="flex-1 pt-18"> </main>
      </div>
    </ErrorBoundary>
  );
}

export default App;
