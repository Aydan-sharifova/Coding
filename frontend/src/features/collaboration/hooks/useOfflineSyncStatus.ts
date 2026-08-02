import { useEffect, useState } from "react";
import type { SignalRYjsProvider } from "../crdt/SignalRYjsProvider";
import type { SyncStatus } from "../crdt/types";

export function useOfflineSyncStatus(provider?: SignalRYjsProvider) {
  const [state, setState] = useState<{ status: SyncStatus; pending: number }>({ status: provider?.status ?? "connecting", pending: 0 });
  useEffect(() => provider?.subscribe((status, pending) => setState({ status, pending })), [provider]);
  return state;
}
