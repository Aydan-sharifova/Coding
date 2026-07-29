import { useState, type CSSProperties } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { AuthMotionScene } from "../components/AuthMotionScene";
import { useAuth } from "../hooks/useAuth";
import type { DemoRole } from "../types/auth";

const personas: Array<{
  role: DemoRole;
  label: string;
  shortLabel: string;
  description: string;
  capabilities: string[];
  note: string;
}> = [
  {
    role: "Owner",
    label: "Project Owner",
    shortLabel: "OW",
    description: "See the complete project experience and guide the workspace.",
    capabilities: ["Manage project settings", "Organize members and tasks", "Use AI with full project context"],
    note: "Project-level owner access",
  },
  {
    role: "Admin",
    label: "Project Admin",
    shortLabel: "AD",
    description: "Coordinate delivery without receiving real platform administration rights.",
    capabilities: ["Manage the Kanban workflow", "Assign work to members", "Review activity and analytics"],
    note: "No system-admin privileges",
  },
  {
    role: "Member",
    label: "Team Member",
    shortLabel: "MB",
    description: "Explore the daily collaborative coding flow as an active developer.",
    capabilities: ["Edit and version project files", "Chat with the workspace", "Complete assigned tasks"],
    note: "Safe contributor access",
  },
];

export function DemoLoginPage() {
  const { demoLogin, session, isInitializing } = useAuth();
  const navigate = useNavigate();
  const [selectedRole, setSelectedRole] = useState<DemoRole | null>(null);
  const [error, setError] = useState<string | null>(null);

  const enterDemo = async (role: DemoRole) => {
    setSelectedRole(role);
    setError(null);
    try {
      const demoSession = await demoLogin({ role });
      navigate(
        demoSession.user.demoProjectId
          ? `/projects/${demoSession.user.demoProjectId}/workspace`
          : "/projects",
        { replace: true },
      );
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : "The demo could not be opened. Please try again.",
      );
      setSelectedRole(null);
    }
  };

  if (!isInitializing && session) {
    return (
      <Navigate
        to={
          session.user.demoProjectId
            ? `/projects/${session.user.demoProjectId}/workspace`
            : "/dashboard"
        }
        replace
      />
    );
  }

  return (
    <main className="demo-entry-page">
      <div className="demo-entry-visual" aria-hidden="true">
        <AuthMotionScene />
        <span className="demo-grid-glow" />
      </div>

      <nav className="demo-entry-nav" aria-label="Demo navigation">
        <Link className="demo-entry-brand" to="/ai">
          <span className="brand-mark">C</span>
          <span>Coding</span>
        </Link>
        <span className="demo-environment-pill">
          <i />
          Isolated demo environment
        </span>
        <div>
          <Link to="/ai">Try guest AI</Link>
          <Link to="/login">Sign in</Link>
        </div>
      </nav>

      <section className="demo-entry-content">
        <header>
          <p className="eyebrow">Nebula Commerce Platform</p>
          <h1>Choose a seat. Enter the live workspace.</h1>
          <p>
            Explore a realistic collaborative project with prepared code,
            conversations, tasks, notifications, and analytics. No password is
            exposed and every change resets automatically.
          </p>
        </header>

        {error && <div className="demo-entry-error" role="alert">{error}</div>}

        <div className="demo-role-grid">
          {personas.map((persona, index) => (
            <article
              className={`demo-role-card demo-role-${persona.role.toLowerCase()}`}
              key={persona.role}
              style={{ "--demo-card-index": index } as CSSProperties}
            >
              <div className="demo-role-card-head">
                <span>{persona.shortLabel}</span>
                <small>{persona.note}</small>
              </div>
              <h2>{persona.label}</h2>
              <p>{persona.description}</p>
              <ul>
                {persona.capabilities.map((capability) => (
                  <li key={capability}><i>✓</i>{capability}</li>
                ))}
              </ul>
              <button
                type="button"
                disabled={selectedRole !== null}
                onClick={() => void enterDemo(persona.role)}
              >
                {selectedRole === persona.role
                  ? "Preparing workspace…"
                  : `Continue as ${persona.role}`}
                <span aria-hidden="true">↗</span>
              </button>
            </article>
          ))}
        </div>

        <footer className="demo-entry-trust">
          <span><i>01</i> Separate database</span>
          <span><i>02</i> Short-lived access</span>
          <span><i>03</i> Automatic reset</span>
          <span><i>04</i> Limited uploads &amp; AI</span>
        </footer>
      </section>
    </main>
  );
}
