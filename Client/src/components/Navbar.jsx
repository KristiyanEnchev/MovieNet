import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Film, Home, Tv } from 'lucide-react';
import { useSelector } from 'react-redux';
import { selectIsAuthenticated } from '../features/auth/authSlice';
import { ThemeToggle } from './shared/ThemeToggle';
import { UserMenu } from './shared/UserMenu';

const navItems = [
  { path: '/', icon: Home, label: 'Home' },
  { path: '/movies', icon: Film, label: 'Movies' },
  { path: '/shows', icon: Tv, label: 'Shows' },
];

export function Navbar() {
  const location = useLocation();
  const isAuthenticated = useSelector(selectIsAuthenticated);

  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-background/20 backdrop-blur-md border-b border-border/40">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <Link
            to="/"
            className="text-xl font-bold text-primary hover:opacity-80 transition-opacity"
          >
            MovieNet
          </Link>

          <div className="flex items-center space-x-4">
            {navItems.map(({ path, icon: Icon, label }) => {
              const isActive = location.pathname === path;
              return (
                <Link
                  key={path}
                  to={path}
                  className={`flex items-center px-3 py-2 rounded-md transition-all ${
                    isActive
                      ? 'text-primary bg-accent/30'
                      : 'text-muted-foreground hover:text-primary hover:bg-accent/20'
                  }`}
                >
                  <Icon className="h-5 w-5" />
                  <span className="ml-2 hidden sm:inline">{label}</span>
                </Link>
              );
            })}
          </div>

          <div className="flex items-center space-x-4">
            <ThemeToggle />
            {isAuthenticated ? (
              <UserMenu />
            ) : (
              <Link
                to="/login"
                className="text-muted-foreground hover:text-primary transition-colors"
              >
                Login
              </Link>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
