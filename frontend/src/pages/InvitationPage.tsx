import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { projectApi } from "../features/projects/api";

export function InvitationPage() {
  const { token = "" } = useParams(); const navigate = useNavigate(); const [pending, setPending] = useState<"accept" | "reject" | null>(null); const [error, setError] = useState<string | null>(null);
  const respond = async (accept: boolean) => { setPending(accept ? "accept" : "reject"); setError(null); try { if (accept) { const result = await projectApi.acceptInvitation(token); navigate(`/projects/${result.projectId}/settings`, { replace: true }); } else { await projectApi.rejectInvitation(token); navigate("/projects", { replace: true }); } } catch (reason) { setError(reason instanceof Error ? reason.message : "Unable to process invitation."); setPending(null); } };
  return <main className="invitation-page"><section className="invitation-card"><span className="brand-mark">C</span><p className="dashboard-date">PROJECT INVITATION</p><h1>You’ve been invited to collaborate</h1><p>Accept to join the project, or reject to dismiss this invitation. Invitations are account-specific and expire automatically.</p>{error && <div className="form-alert">{error}</div>}<div><button className="ui-button ghost" disabled={Boolean(pending)} onClick={() => respond(false)}>{pending === "reject" ? "Rejecting…" : "Reject"}</button><button className="ui-button primary" disabled={Boolean(pending)} onClick={() => respond(true)}>{pending === "accept" ? "Joining…" : "Accept invitation"}</button></div></section></main>;
}
