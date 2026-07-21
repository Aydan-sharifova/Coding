import { useEffect, type PropsWithChildren, type ReactNode } from "react";

interface DialogProps extends PropsWithChildren { open: boolean; title: string; description?: string; footer?: ReactNode; onClose: () => void; }

export function Dialog({ open, title, description, footer, onClose, children }: DialogProps) {
  useEffect(() => {
    if (!open) return;
    const close = (event: KeyboardEvent) => { if (event.key === "Escape") onClose(); };
    window.addEventListener("keydown", close);
    return () => window.removeEventListener("keydown", close);
  }, [open, onClose]);
  if (!open) return null;
  return <div className="dialog-layer" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget) onClose(); }}><section className="dialog-card" role="dialog" aria-modal="true" aria-labelledby="dialog-title"><button className="dialog-close" onClick={onClose} aria-label="Close">×</button><header><h2 id="dialog-title">{title}</h2>{description && <p>{description}</p>}</header><div className="dialog-body">{children}</div>{footer && <footer>{footer}</footer>}</section></div>;
}

export function ConfirmDialog({ open, title, description, confirmLabel = "Confirm", destructive, pending, onClose, onConfirm }: { open: boolean; title: string; description: string; confirmLabel?: string; destructive?: boolean; pending?: boolean; onClose: () => void; onConfirm: () => void }) {
  return <Dialog open={open} title={title} description={description} onClose={onClose} footer={<><button className="ui-button ghost" onClick={onClose}>Cancel</button><button className={`ui-button ${destructive ? "danger" : "primary"}`} disabled={pending} onClick={onConfirm}>{pending ? "Working…" : confirmLabel}</button></>}><div className="confirmation-note">This action is enforced and validated by the server.</div></Dialog>;
}
