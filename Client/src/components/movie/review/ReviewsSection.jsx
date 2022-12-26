import { ReviewCard } from './ReviewCard';

export default function ReviewsSection({ reviews }) {
  if (!reviews?.length) return null;

  return (
    <section className="mb-10">
      <h2 className="text-2xl font-bold mb-6">Reviews</h2>
      <div className="space-y-6">
        {reviews.map((review) => (
          <ReviewCard key={review.id} review={review} />
        ))}
      </div>
    </section>
  );
}
