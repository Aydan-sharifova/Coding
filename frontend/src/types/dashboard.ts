export interface ProjectSummary {
  id: string;
  name: string;
  description: string;
  language: string;
  progress: number;
  updatedAt: string;
  color: string;
}

export interface ActivityItem {
  id: string;
  title: string;
  detail: string;
  time: string;
  tone: "purple" | "blue" | "green" | "orange";
}
