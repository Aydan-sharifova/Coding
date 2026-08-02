import type { SignalRYjsProvider } from "../crdt/SignalRYjsProvider";
import { useOfflineSyncStatus } from "../hooks/useOfflineSyncStatus";

export function OfflineChangesIndicator({ provider }: { provider?: SignalRYjsProvider }) {
  const { pending } = useOfflineSyncStatus(provider); return pending ? <span className="offline-changes">{pending} offline update{pending === 1 ? "" : "s"}</span> : null;
}
