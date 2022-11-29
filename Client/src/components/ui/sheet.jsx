import React from 'react';
import { X } from 'lucide-react';
import { cn } from '../../lib/utils';

const Sheet = ({ isOpen, onClose, children }) => {
  if (!isOpen) return null;

  return (
    <>
      {/* Overlay */}
      <div
        className="fixed inset-0 z-50 bg-black/50 backdrop-blur-sm"
        onClick={onClose}
      />
      {/* Sheet */}
      <div className="fixed inset-y-0 right-0 z-50 w-full max-w-sm border-l bg-background p-6 shadow-lg">
        <button
          onClick={onClose}
          className="absolute right-4 top-4 rounded-sm opacity-70 ring-offset-background transition-opacity hover:opacity-100"
        >
          <X className="h-4 w-4" />
          <span className="sr-only">Close</span>
        </button>
        {children}
      </div>
    </>
  );
};

const SheetContent = ({ children, className, ...props }) => (
  <div className={cn('relative', className)} {...props}>
    {children}
  </div>
);

const SheetHeader = ({ className, ...props }) => (
  <div
    className={cn(
      'flex flex-col space-y-2 text-center sm:text-left',
      className
    )}
    {...props}
  />
);

const SheetTrigger = ({ children, onClick }) => {
  return React.cloneElement(children, { onClick });
};

export { Sheet, SheetContent, SheetHeader, SheetTrigger };
