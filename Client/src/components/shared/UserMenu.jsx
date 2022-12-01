import React, { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useLogoutMutation } from '../../features/auth/authApi';
import { useSelector, useDispatch } from 'react-redux';
import { selectCurrentUser } from '../../features/auth/authSlice';
import { clearCredentials } from '../../features/auth/authSlice';

export function UserMenu() {
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef(null);
  const [logout] = useLogoutMutation();
  const user = useSelector(selectCurrentUser);
  const dispatch = useDispatch();

  useEffect(() => {
    function handleClickOutside(event) {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleLogout = async () => {
    try {
      if (user?.email) {
        await logout(user.email);
        dispatch(clearCredentials());
      }
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  return (
    <div className="relative" ref={menuRef}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center space-x-2 text-muted-foreground hover:text-primary"
      >
        <div className="w-8 h-8 rounded-full bg-primary text-primary-foreground flex items-center justify-center">
          {user?.firstName?.[0] || 'U'}
        </div>
      </button>

      {isOpen && (
        <div className="absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-background border border-border">
          <Link
            to="/watchlist"
            className="block px-4 py-2 text-sm text-foreground hover:bg-accent"
            onClick={() => setIsOpen(false)}
          >
            Watchlist
          </Link>
          <button
            onClick={() => {
              handleLogout();
              setIsOpen(false);
            }}
            className="block w-full text-left px-4 py-2 text-sm text-foreground hover:bg-accent"
          >
            Logout
          </button>
        </div>
      )}
    </div>
  );
}
