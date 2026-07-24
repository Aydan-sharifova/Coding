import { Line } from "react-chartjs-2";
import { CategoryScale, Chart as ChartJS, Filler, Legend, LinearScale, LineElement, PointElement, Tooltip } from "chart.js";
import { useTheme } from "../hooks/useTheme";

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Filler, Tooltip, Legend);

export function WeeklyProgressChart({ points }: { points: Array<{ date: string; contributions: number }> }) {
  const { theme } = useTheme();
  const grid = theme === "dark" ? "rgba(148, 163, 184, .12)" : "rgba(100, 116, 139, .1)";
  const text = theme === "dark" ? "#94a3b8" : "#7b8497";
  return (
    <div className="chart-wrap">
      <Line
        data={{
          labels: points.map((point) => new Date(`${point.date}T00:00:00`).toLocaleDateString(undefined, { weekday: "short" })),
          datasets: [{ label: "Activity", data: points.map((point) => point.contributions), borderColor: "#6c5ce7", backgroundColor: "rgba(108, 92, 231, .12)", pointBackgroundColor: "#6c5ce7", pointBorderColor: theme === "dark" ? "#171b29" : "#fff", pointBorderWidth: 3, pointRadius: 4, tension: .4, fill: true }],
        }}
        options={{
          responsive: true,
          maintainAspectRatio: false,
          interaction: { intersect: false, mode: "index" },
          plugins: { legend: { display: false }, tooltip: { padding: 12, displayColors: false, backgroundColor: theme === "dark" ? "#2a3042" : "#172033", callbacks: { label: (item) => `${item.formattedValue} contributions` } } },
          scales: { x: { border: { display: false }, grid: { display: false }, ticks: { color: text, font: { family: "DM Sans", size: 11 } } }, y: { beginAtZero: true, border: { display: false }, grid: { color: grid }, ticks: { color: text, stepSize: 5, font: { family: "DM Sans", size: 11 } } } },
        }}
      />
    </div>
  );
}
