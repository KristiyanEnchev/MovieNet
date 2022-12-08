// components/shared/PageLayout.js
import React from 'react';
import Navbar from '../Navbar';
import { cn } from '../../lib/utils';
import SearchBar from './SearchBar';
import { Footer } from './Footer';

export default function PageLayout({
  children,
  className,
  containerClassName,
  fullWidth = false,
  showSearch = false,
  onSearchResults = null,
  heroContent = null,
}) {
  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground">
      <Navbar />
      <main className={cn('flex-1', className)}>
        {heroContent}
        <div
          className={cn(
            fullWidth ? '' : 'container mx-auto px-4',
            containerClassName
          )}
        >
          {showSearch && (
            <div className="max-w-2xl mx-auto mb-8">
              <SearchBar
                mediaType="multi"
                onSearchResults={onSearchResults}
                className="w-full"
              />
            </div>
          )}
          {children}
        </div>
      </main>
      <Footer />
    </div>
  );
}
