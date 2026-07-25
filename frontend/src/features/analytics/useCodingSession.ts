import { useEffect } from "react";
import { analyticsApi } from "./api";

export function useCodingSession(projectId?: string, fileId?: string) {
  useEffect(() => {
    if (!projectId || !fileId) return;
    let disposed = false, sessionId: string | undefined, interval: number | undefined;
    void analyticsApi.startSession(projectId, fileId).then((result) => {
      if (disposed) { void analyticsApi.endSession(result.sessionId); return; }
      sessionId = result.sessionId;
      interval = window.setInterval(() => {
        if (document.visibilityState === "visible" && sessionId) void analyticsApi.heartbeat(sessionId);
      }, 60_000);
    }).catch(() => undefined);
    return () => {
      disposed = true;
      if (interval) window.clearInterval(interval);
      if (sessionId) void analyticsApi.endSession(sessionId);
    };
  }, [projectId, fileId]);
}
