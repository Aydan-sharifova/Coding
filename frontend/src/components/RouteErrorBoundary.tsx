import { Component, type ErrorInfo, type ReactNode } from "react";

interface Props { children: ReactNode; }
interface State { error?: Error; }

export class RouteErrorBoundary extends Component<Props, State> {
  state: State = {};
  static getDerivedStateFromError(error: Error): State { return { error }; }
  componentDidCatch(error: Error, info: ErrorInfo) { console.error("Route rendering failed", error, info); }
  render() {
    if (!this.state.error) return this.props.children;
    return <main className="route-error"><div className="brand-mark">C</div><h1>This workspace could not be displayed</h1><p>{this.state.error.message || "An unexpected interface error occurred."}</p><div><button className="ui-button primary" onClick={() => window.location.reload()}>Reload workspace</button><button className="ui-button ghost" onClick={() => { window.location.href = "/projects"; }}>Back to projects</button></div></main>;
  }
}
