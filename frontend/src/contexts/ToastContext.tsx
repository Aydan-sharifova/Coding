import { createContext, useCallback, useContext, useMemo, useState, type PropsWithChildren } from "react";

type ToastTone = "success" | "error";
interface Toast { id: number; message: string; tone: ToastTone; }
const ToastContext = createContext<{ show: (message: string, tone?: ToastTone) => void } | null>(null);

export function ToastProvider({ children }: PropsWithChildren) {
  const [toasts, setToasts] = useState<Toast[]>([]);
  const dismiss=useCallback((id:number)=>setToasts((items)=>items.filter((item)=>item.id!==id)),[]);
  const show = useCallback((message: string, tone: ToastTone = "success") => {
    const id = Date.now()+Math.random(); setToasts((items) => [...items.slice(-3), { id, message, tone }]);
    window.setTimeout(() => dismiss(id), 4500);
  }, [dismiss]);
  const value = useMemo(() => ({ show }), [show]);
  return <ToastContext.Provider value={value}>{children}<div className="toast-region" aria-live="polite" aria-label="Notifications">{toasts.map((toast) => <div className={`toast ${toast.tone}`} role={toast.tone==="error"?"alert":"status"} key={toast.id}><span>{toast.message}</span><button aria-label="Dismiss notification" onClick={()=>dismiss(toast.id)}>×</button></div>)}</div></ToastContext.Provider>;
}

export function useToast() { const value = useContext(ToastContext); if (!value) throw new Error("useToast must be inside ToastProvider."); return value; }
