import { Link, Outlet, useLocation } from "react-router-dom";
import { AuthMotionScene } from "../components/AuthMotionScene";

export function AuthLayout() {
  const { pathname } = useLocation();
  const isLogin = pathname === "/login";
  const isRegister = pathname === "/register";

  return (
    <main className="auth-shell">
      <section className="brand-panel" aria-label="Coding platform introduction">
        <AuthMotionScene />
        <Link className="brand" to="/ai" aria-label="Coding home">
          <span className="brand-mark" aria-hidden="true">C</span>
          <span>Coding</span>
        </Link>
        <div className="brand-copy">
          <p className="eyebrow">Build together. Think in motion.</p>
          <h1>One intelligent space from first idea to shipped code.</h1>
          <p>
            Start with Aydan AI for free. Create an account when you are ready
            for project memory, real-time collaboration, and a complete
            engineering workspace.
          </p>
          <div className="auth-capability-row" aria-label="Platform capabilities">
            <span><i>✦</i> Guest AI</span>
            <span><i>⌘</i> Live editor</span>
            <span><i>↗</i> Team flow</span>
          </div>
        </div>
        <div className="auth-live-card auth-live-card-one" aria-hidden="true">
          <span>AI</span>
          <div><b>Context connected</b><small>Reasoning across your project</small></div>
        </div>
        <div className="auth-live-card auth-live-card-two" aria-hidden="true">
          <span>03</span>
          <div><b>Team online</b><small>Changes synchronized live</small></div>
        </div>
        <p className="trust-note"><span aria-hidden="true">●</span> Local-first AI · secure sessions</p>
      </section>

      <section className="form-panel">
        <div className="mobile-brand">
          <Link to="/ai"><span className="brand-mark">C</span> Coding</Link>
          <Link to="/ai">Try AI free</Link>
        </div>
        <nav className="auth-mode-nav" aria-label="Account access">
          <Link className={isLogin ? "active" : ""} to="/login">Sign in</Link>
          <Link className={isRegister ? "active" : ""} to="/register">Create account</Link>
          <Link to="/ai">Guest AI <span>↗</span></Link>
        </nav>
        <div className="auth-card">
          <Outlet />
        </div>
        <p className="auth-switch">
          {isLogin ? "New to Coding?" : "Already have an account?"}{" "}
          <Link to={isLogin ? "/register" : "/login"}>
            {isLogin ? "Create an account" : "Sign in"}
          </Link>
          <span aria-hidden="true"> · </span>
          <Link to="/ai">Continue without an account</Link>
        </p>
      </section>
    </main>
  );
}
