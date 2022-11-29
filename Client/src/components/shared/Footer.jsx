export function Footer() {
  const social = [
    {
      id: 1,
      url: 'https://www.facebook.com',
      icon: <i className="fab fa-facebook-f"></i>,
    },
    {
      id: 2,
      url: 'https://www.twitter.com',
      icon: <i className="fab fa-twitter"></i>,
    },
    {
      id: 3,
      url: 'https://www.linkedin.com',
      icon: <i className="fab fa-linkedin"></i>,
    },
  ];

  return (
    <footer className="mt-auto py-4 bg-background border-t">
      <div className="container mx-auto px-4">
        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <div className="flex items-center space-x-4">
            {social.map(({ id, url, icon }) => (
              <a
                key={id}
                href={url}
                target="_blank"
                rel="noreferrer"
                className="text-muted-foreground hover:text-foreground transition-colors"
              >
                {icon}
              </a>
            ))}
          </div>
          <div className="text-sm text-muted-foreground">
            {new Date().getFullYear()} MovieNet by{' '}
            <a
              href="https://kristiyan-enchev-website.web.app/"
              target="_blank"
              rel="noreferrer"
              className="hover:text-foreground transition-colors"
            >
              Kristiyan Enchev
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
}
