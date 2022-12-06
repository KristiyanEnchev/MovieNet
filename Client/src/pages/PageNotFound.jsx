import React from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';

const PageNotFound = () => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <div className="max-w-md w-full bg-card/50 backdrop-blur-xl rounded-2xl shadow-xl overflow-hidden border border-border">
        <div className="p-8">
          <motion.div
            initial={{ scale: 0.8, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            transition={{ duration: 0.5 }}
            className="text-center"
          >
            {/* 404 Icon */}
            <div className="mb-6 text-9xl font-bold bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text">
              404
            </div>

            <h1 className="text-3xl font-bold mb-4">Page Not Found</h1>
            <p className="text-muted-foreground mb-8">
              The page you are looking for doesn't exist or has been moved.
            </p>

            <div className="flex gap-4">
              <Link
                to="/"
                className="flex-1 py-3 px-4 bg-gradient-to-r from-primary to-primary/80 text-primary-foreground
                  font-bold rounded-lg shadow-lg hover:from-primary/90 hover:to-primary/70
                  focus:outline-none focus:ring-2 focus:ring-primary/30 focus:ring-offset-2
                  focus:ring-offset-background transition-all duration-200 transform hover:scale-[1.02]
                  active:scale-[0.98] text-center"
              >
                Go Home
              </Link>
              <button
                onClick={() => window.history.back()}
                className="flex-1 py-3 px-4 bg-muted text-muted-foreground font-bold rounded-lg
                  shadow-lg hover:bg-muted/80 focus:outline-none focus:ring-2 focus:ring-primary/30
                  focus:ring-offset-2 focus:ring-offset-background transition-all duration-200
                  transform hover:scale-[1.02] active:scale-[0.98]"
              >
                Go Back
              </button>
            </div>
          </motion.div>
        </div>
      </div>
    </div>
  );
};

export default PageNotFound;
