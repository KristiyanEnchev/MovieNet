import PropTypes from 'prop-types';
import { Component } from 'react';
import { Link } from 'react-router-dom';

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null, errorInfo: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    console.error('ErrorBoundary caught an error', error, errorInfo);
    this.setState({ error, errorInfo });
  }

  handleReload = () => {
    window.location.reload();
  };

  render() {
    if (this.state.hasError) {
      return (
        <div className="min-h-screen flex items-center justify-center bg-background p-4">
          <div className="max-w-md w-full bg-card/50 backdrop-blur-xl rounded-2xl shadow-xl overflow-hidden border border-border p-8">
            <h1 className="text-3xl font-bold text-center bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text mb-4">
              Oops! Something went wrong
            </h1>
            <p className="text-muted-foreground text-center mb-6">
              An unexpected error has occurred. We apologize for the
              inconvenience.
            </p>

            <div className="space-y-4">
              <details className="bg-muted/50 rounded-lg p-4 text-left text-sm text-muted-foreground">
                <summary className="cursor-pointer font-medium">
                  Error Details
                </summary>
                <div className="mt-2 whitespace-pre-wrap">
                  {this.state.error?.toString()}
                  <br />
                  {this.state.errorInfo?.componentStack}
                </div>
              </details>

              <div className="flex gap-4">
                <button
                  onClick={this.handleReload}
                  className="flex-1 py-3 px-4 bg-gradient-to-r from-primary to-primary/80 text-primary-foreground
                    font-bold rounded-lg shadow-lg hover:from-primary/90 hover:to-primary/70
                    focus:outline-none focus:ring-2 focus:ring-primary/30 focus:ring-offset-2
                    focus:ring-offset-background transition-all duration-200 transform hover:scale-[1.02] active:scale-[0.98]"
                >
                  Reload Page
                </button>
                <Link
                  to="/"
                  className="flex-1 py-3 px-4 bg-muted text-muted-foreground font-bold rounded-lg
                    shadow-lg hover:bg-muted/80 focus:outline-none focus:ring-2 focus:ring-primary/30
                    focus:ring-offset-2 focus:ring-offset-background transition-all duration-200
                    transform hover:scale-[1.02] active:scale-[0.98] text-center"
                >
                  Go Home
                </Link>
              </div>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

ErrorBoundary.propTypes = {
  children: PropTypes.node.isRequired,
};

export default ErrorBoundary;
