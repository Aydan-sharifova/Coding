import { Icon, type IconName } from "./Icon";

interface StatCardProps {
  label: string;
  value: string;
  change: string;
  icon: IconName;
  tone: string;
  changePercent?: number;
}

export function StatCard({ label, value, change, icon, tone, changePercent = 0 }: StatCardProps) {
  return (
    <article className="stat-card">
      <div className={`stat-icon ${tone}`}><Icon name={icon} /></div>
      <div>
        <p>{label}</p>
        <strong>{value}</strong>
        <span className={`stat-change ${changePercent < 0 ? "negative" : ""}`}><Icon name="trend" /> {changePercent !== 0 ? `${changePercent > 0 ? "+" : ""}${changePercent}% ` : ""}{change}</span>
      </div>
    </article>
  );
}
