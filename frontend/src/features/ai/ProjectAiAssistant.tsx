import { useState } from "react";
import { Dialog } from "../../components/ui/Dialog";
import { AiAssistantPanel } from "./AiAssistantPanel";

interface ProjectAiAssistantProps {
  projectId: string;
  contextLabel: string;
  context: string;
}

export function ProjectAiAssistant({ projectId, contextLabel, context }: ProjectAiAssistantProps) {
  const [open, setOpen] = useState(false);
  return (
    <>
      <button className="ui-button ai-launch-button" onClick={() => setOpen(true)}>✦ Ask AI</button>
      <Dialog open={open} title="Project AI assistant" description={`Context: ${contextLabel}`} onClose={() => setOpen(false)}>
        <div className="project-ai-dialog">
          <AiAssistantPanel
            projectId={projectId}
            fileName={contextLabel}
            selectedCode={context.slice(0, 10_000)}
            onApplySuggestion={() => undefined}
          />
        </div>
      </Dialog>
    </>
  );
}
