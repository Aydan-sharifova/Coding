import type { SignalRYjsProvider } from "../crdt/SignalRYjsProvider";
import { useOfflineSyncStatus } from "../hooks/useOfflineSyncStatus";

const labels = { connecting: "Connecting", connected: "Connected", reconnecting: "Reconnecting", offline: "Offline changes", synchronizing: "Synchronizing", synchronized: "Synchronized", failed: "Synchronization failed" } as const;
export function CollaborationStatus({ provider }: { provider?: SignalRYjsProvider }) {
  const { status, pending } = useOfflineSyncStatus(provider);
  return <span className={`crdt-status ${status}`} aria-live="polite"><i />{labels[status]}{pending > 0 ? ` (${pending})` : ""}</span>;
}
