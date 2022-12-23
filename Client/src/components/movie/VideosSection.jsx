export default function VideosSection({ videos }) {
  if (!videos?.length) return null;

  return (
    <section className="mb-10">
      <h2 className="text-2xl font-bold mb-6">Videos</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {videos.map((video) => (
          <div
            key={video.id}
            className="aspect-video rounded-lg overflow-hidden bg-muted"
          >
            <iframe
              src={`https://www.youtube.com/embed/${video.key}`}
              title={video.name}
              className="w-full h-full"
              allowFullScreen
            />
          </div>
        ))}
      </div>
    </section>
  );
}
