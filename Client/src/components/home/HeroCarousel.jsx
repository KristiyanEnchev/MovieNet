import React, { useState, useEffect, useCallback } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Link } from 'react-router-dom';
import { Button } from '../ui/button';
import { cn } from '../../lib/utils';

const AUTO_SLIDE_INTERVAL = 5000; // 5 seconds

export function HeroCarousel({ items = [] }) {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [direction, setDirection] = useState(0);
  const [isPaused, setIsPaused] = useState(false);

  const carouselItems = items.slice(0, 5).map((item) => ({
    ...item,
    backdrop_path: item.backdropPath,
    overview:
      item.overview?.slice(0, 200) + (item.overview?.length > 200 ? '...' : ''),
  }));

  const slideVariants = {
    enter: (direction) => ({
      x: direction > 0 ? 1000 : -1000,
      opacity: 0,
    }),
    center: {
      zIndex: 1,
      x: 0,
      opacity: 1,
    },
    exit: (direction) => ({
      zIndex: 0,
      x: direction < 0 ? 1000 : -1000,
      opacity: 0,
    }),
  };

  const swipeConfidenceThreshold = 10000;
  const swipePower = (offset, velocity) => {
    return Math.abs(offset) * velocity;
  };

  const paginate = useCallback(
    (newDirection) => {
      setDirection(newDirection);
      setCurrentIndex(
        (prevIndex) =>
          (prevIndex + newDirection + carouselItems.length) %
          carouselItems.length
      );
    },
    [carouselItems.length]
  );

  useEffect(() => {
    if (!isPaused) {
      const timer = setInterval(() => {
        paginate(1);
      }, AUTO_SLIDE_INTERVAL);

      return () => clearInterval(timer);
    }
  }, [isPaused, paginate]);

  if (!carouselItems?.length) return null;

  const currentItem = carouselItems[currentIndex];

  return (
    <div
      className="relative h-[60vh] overflow-hidden rounded-b-3xl"
      onMouseEnter={() => setIsPaused(true)}
      onMouseLeave={() => setIsPaused(false)}
    >
      <AnimatePresence initial={false} custom={direction}>
        <motion.div
          key={currentIndex}
          className="absolute inset-0"
          custom={direction}
          variants={slideVariants}
          initial="enter"
          animate="center"
          exit="exit"
          transition={{
            x: { type: 'spring', stiffness: 300, damping: 30 },
            opacity: { duration: 0.2 },
          }}
          drag="x"
          dragConstraints={{ left: 0, right: 0 }}
          dragElastic={1}
          onDragEnd={(e, { offset, velocity }) => {
            const swipe = swipePower(offset.x, velocity.x);

            if (swipe < -swipeConfidenceThreshold) {
              paginate(1);
            } else if (swipe > swipeConfidenceThreshold) {
              paginate(-1);
            }
          }}
        >
          {/* Background Image */}
          <div
            className="absolute inset-0 bg-cover bg-center rounded-b-3xl"
            style={{
              backgroundImage: `url(${currentItem.backdrop_path})`,
            }}
          >
            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/20 to-transparent rounded-b-3xl" />
          </div>

          {/* Content */}
          <motion.div
            className="absolute bottom-0 left-0 right-0 p-8"
            initial={{ y: 20, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.2 }}
          >
            <div className="container mx-auto">
              <h2 className="mb-2 text-4xl font-bold text-white">
                {currentItem.title}
              </h2>
              <p className="mb-4 max-w-2xl text-lg text-gray-200">
                {currentItem.overview}
              </p>
              <Link
                to={`${
                  currentItem.mediaType === 'movie' ? '/movies' : '/shows'
                }/${currentItem.tmdbId}/${currentItem.mediaType}`}
              >
                <Button size="lg" className="bg-primary hover:bg-primary/90">
                  View Details
                </Button>
              </Link>
            </div>
          </motion.div>
        </motion.div>
      </AnimatePresence>

      {/* Navigation Buttons */}
      <div className="absolute inset-0 flex items-center justify-between px-4 pointer-events-none">
        <motion.button
          className="z-20 p-3 rounded-full bg-black/50 hover:bg-black/70 transition-colors pointer-events-auto"
          onClick={() => paginate(-1)}
          whileHover={{ scale: 1.1 }}
          whileTap={{ scale: 0.9 }}
        >
          <ChevronLeft className="h-8 w-8 text-white" />
        </motion.button>
        <motion.button
          className="z-20 p-3 rounded-full bg-black/50 hover:bg-black/70 transition-colors pointer-events-auto"
          onClick={() => paginate(1)}
          whileHover={{ scale: 1.1 }}
          whileTap={{ scale: 0.9 }}
        >
          <ChevronRight className="h-8 w-8 text-white" />
        </motion.button>
      </div>

      {/* Indicators */}
      <div className="absolute bottom-8 left-1/2 -translate-x-1/2 flex items-center space-x-3 z-20">
        {carouselItems.map((_, index) => (
          <motion.button
            key={index}
            className={cn(
              'h-3 rounded-full bg-white/50 hover:bg-white transition-all',
              index === currentIndex ? 'w-8 bg-primary' : 'w-3'
            )}
            onClick={() => {
              const newDirection = index > currentIndex ? 1 : -1;
              setDirection(newDirection);
              setCurrentIndex(index);
            }}
            whileHover={{ scale: 1.2 }}
            whileTap={{ scale: 0.9 }}
          />
        ))}
      </div>
    </div>
  );
}
