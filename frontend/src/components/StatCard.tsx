import { Icon, type IconName } from "./Icon";

interface StatCardProps {
  label: string;
  value: string;
  change: string;
  icon: IconName;
  tone: string;
}

export function StatCard({ label, value, change, icon, tone }: StatCardProps) {
  return (
    <article className="stat-card">
      <div className={`stat-icon ${tone}`}><Icon name={icon} /></div>
      <div>
        <p>{label}</p>
        <strong>{value}</strong>
        <span className="stat-change"><Icon name="trend" /> {change}</span>
      </div>
    </article>
  );
}
