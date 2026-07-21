import { Link, Outlet, useLocation } from "react-router-dom";

export function AuthLayout() {
  const { pathname } = useLocation();
  const isLogin = pathname === "/login";

  return (
    <main className="auth-shell">
      <section className="brand-panel" aria-label="Coding platform introduction">
        <Link className="brand" to="/login" aria-label="Coding home">
          <span className="brand-mark" aria-hidden="true">C</span>
          <span>Coding</span>
        </Link>
        <div className="brand-copy">
          <p className="eyebrow">Build together. Ship with confidence.</p>
          <h1>Your focused space for collaborative development.</h1>
          <p>Secure access, clear roles, and a workspace designed to keep engineering teams moving.</p>
        </div>
        <p className="trust-note"><span aria-hidden="true">●</span> Secure session protection enabled</p>
      </section>

      <section className="form-panel">
        <div className="mobile-brand"><span className="brand-mark">C</span> Coding</div>
        <div className="auth-card">
          <Outlet />
        </div>
        <p className="auth-switch">
          {isLogin ? "New to Coding?" : "Already have an account?"}{" "}
          <Link to={isLogin ? "/register" : "/login"}>
            {isLogin ? "Create an account" : "Sign in"}
          </Link>
        </p>
      </section>
    </main>
  );
}
