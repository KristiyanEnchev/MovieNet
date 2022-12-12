import React, { useState, useEffect } from 'react';

export function Pagination({ currentPage, onPageChange }) {
  const [inputPage, setInputPage] = useState(currentPage);

  useEffect(() => {
    setInputPage(currentPage);
  }, [currentPage]);

  const handleInputChange = (e) => {
    const value = e.target.value;
    if (value === '' || /^\d+$/.test(value)) {
      setInputPage(value);
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const newPage = parseInt(inputPage, 10);
    if (newPage && newPage > 0) {
      onPageChange(newPage);
    } else {
      setInputPage(currentPage);
    }
  };

  const handleBlur = () => {
    if (!inputPage) {
      setInputPage(currentPage);
    }
  };

  return (
    <div className="flex justify-center items-center gap-2 mt-8">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
        className="px-4 py-2 bg-primary text-primary-foreground rounded-lg disabled:opacity-50"
      >
        Previous
      </button>
      <form onSubmit={handleSubmit} className="flex items-center">
        <input
          type="text"
          value={inputPage}
          onChange={handleInputChange}
          onBlur={handleBlur}
          className="w-16 text-center px-2 py-2 rounded-lg bg-accent text-foreground"
          aria-label="Page number"
        />
      </form>
      <button
        onClick={() => onPageChange(currentPage + 1)}
        className="px-4 py-2 bg-primary text-primary-foreground rounded-lg"
      >
        Next
      </button>
    </div>
  );
}
