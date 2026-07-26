import type { ReactNode } from "react";

export function LoadingSpinner({ label = "Loading", size = "md" }: { label?: string; size?: "sm"|"md"|"lg" }) {
  return <span className={`loading-spinner ${size}`} role="status" aria-live="polite"><span aria-hidden="true" /> <span className="sr-only">{label}</span></span>;
}

export function LoadingState({ label = "Loading…" }: { label?: string }) {
  return <div className="feature-state" role="status" aria-live="polite"><LoadingSpinner label={label} /> <span>{label}</span></div>;
}

export function PageSkeleton({ cards = 4 }: { cards?: number }) {
  return <main className="page-skeleton" aria-busy="true" aria-label="Loading page"><div className="skeleton-line heading" /> <div className="skeleton-line copy" /><div className="skeleton-grid">{Array.from({length:cards},(_,i)=><div className="skeleton-card" key={i} />)}</div></main>;
}

export function TableSkeleton({ rows = 6, columns = 5 }: { rows?: number; columns?: number }) {
  return <div className="table-skeleton" role="status" aria-label="Loading table" aria-busy="true">{Array.from({length:rows},(_,row)=><div key={row}>{Array.from({length:columns},(_,col)=><span key={col} />)}</div>)}</div>;
}

export function EmptyState({ title, description, action, icon = "◇" }: { title: string; description: string; action?: ReactNode; icon?: ReactNode }) {
  return <section className="feature-state empty" aria-labelledby="empty-state-title"><span className="state-icon" aria-hidden="true">{icon}</span><strong id="empty-state-title">{title}</strong><p>{description}</p>{action}</section>;
}

export function RetryButton({ onRetry, pending, label = "Try again" }: { onRetry:()=>void; pending?:boolean; label?:string }) {
  return <button type="button" className="ui-button ghost" disabled={pending} onClick={onRetry}>{pending?<><LoadingSpinner size="sm" label="Retrying" /> Retrying…</>:label}</button>;
}

export function ErrorState({ message, retry, title = "Something went wrong", pending }: { message: string; retry?: () => void; title?:string; pending?:boolean }) {
  return <section className="feature-state error" role="alert" aria-labelledby="error-state-title"><span className="state-icon" aria-hidden="true">!</span><strong id="error-state-title">{title}</strong><p>{message}</p>{retry && <RetryButton onRetry={retry} pending={pending} />}</section>;
}

export function FormError({ id, message }: { id?:string; message?:string }) {
  return message ? <span className="field-error" id={id} role="alert">{message}</span> : null;
}

export function ConnectionBanner({ state, onRetry }: { state:"connected"|"reconnecting"|"disconnected"|"failed"; onRetry?:()=>void }) {
  if (state==="connected") return null;
  const copy={reconnecting:"Connection interrupted. Reconnecting…",disconnected:"You are offline. Changes will remain local until connection returns.",failed:"Real-time connection failed."}[state];
  return <div className={`connection-banner ${state}`} role="status" aria-live="assertive"><span aria-hidden="true" /> <p>{copy}</p>{state==="failed"&&onRetry&&<RetryButton onRetry={onRetry} label="Reconnect" />}</div>;
}

export function PermissionDenied({ onBack }: { onBack?:()=>void }) {
  return <section className="permission-denied" role="alert"><span className="state-icon" aria-hidden="true">🔒</span><h1>Permission denied</h1><p>You do not have access to this resource. Ask a project owner or administrator if you believe this is a mistake.</p>{onBack&&<button className="ui-button primary" onClick={onBack}>Go back</button>}</section>;
}
