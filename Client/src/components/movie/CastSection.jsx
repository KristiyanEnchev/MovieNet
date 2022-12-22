export default function CastSection({ cast }) {
  if (!cast?.length) return null;

  return (
    <section className="mb-10">
      <h2 className="text-2xl font-bold mb-6">Cast</h2>
      <div className="relative">
        <div className="flex overflow-x-auto space-x-4 pb-4 scrollbar-hide">
          {cast.map((person) => (
            <div key={person.id} className="flex-none w-32">
              <div className="space-y-2">
                <div className="aspect-[2/3] rounded-lg overflow-hidden bg-muted">
                  <img
                    src={`https://image.tmdb.org/t/p/w185${person.profile_path}`}
                    alt={person.name}
                    className="w-full h-full object-cover"
                    loading="lazy"
                    onError={(e) => {
                      e.target.src =
                        'https://via.placeholder.com/185x278?text=No+Image';
                    }}
                  />
                </div>
                <div className="text-center">
                  <p className="font-semibold text-sm truncate">
                    {person.name}
                  </p>
                  <p className="text-xs text-muted-foreground truncate">
                    {person.character}
                  </p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
