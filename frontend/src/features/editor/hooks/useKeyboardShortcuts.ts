import { useEffect } from "react";

export function useKeyboardShortcuts({ save, close, quickOpen }: { save: () => void; close: () => void; quickOpen: () => void }) {
  useEffect(() => { const handler = (event: KeyboardEvent) => { if (!(event.metaKey || event.ctrlKey)) return; const key = event.key.toLowerCase(); if (key === "s") { event.preventDefault(); save(); } if (key === "w") { event.preventDefault(); close(); } if (key === "p") { event.preventDefault(); quickOpen(); } }; window.addEventListener("keydown", handler); return () => window.removeEventListener("keydown", handler); }, [save, close, quickOpen]);
}
