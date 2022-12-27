import React from 'react';
import { Star } from 'lucide-react';

export function ReviewCard({ userName, rating, comment, createdAt }) {
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffTime = Math.abs(now - date);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays === 0) return 'Today';
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays} days ago`;
    if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
    if (diffDays < 365) return `${Math.floor(diffDays / 30)} months ago`;
    return `${Math.floor(diffDays / 365)} years ago`;
  };

  return (
    <div className="p-4 rounded-lg bg-card">
      <div className="flex items-center justify-between mb-2">
        <p className="font-medium">{userName}</p>
        <div className="flex items-center">
          <Star className="w-4 h-4 text-yellow-400 fill-yellow-400" />
          <span className="ml-1">{rating.toFixed(1)}</span>
        </div>
      </div>
      <p className="text-muted-foreground">{comment}</p>
      <p className="text-sm text-muted-foreground mt-2">
        {formatDate(createdAt)}
      </p>
    </div>
  );
}
