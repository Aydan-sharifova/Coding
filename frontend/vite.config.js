import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173,
        strictPort: true,
        proxy: {
            "/api": {
                target: "http://localhost:5192",
                changeOrigin: true,
            },
            "/health": {
                target: "http://localhost:5192",
                changeOrigin: true,
            },
            "/uploads": {
                target: "http://localhost:5192",
                changeOrigin: true,
            },
            "/hubs": {
                target: "http://localhost:5192",
                changeOrigin: true,
                ws: true,
            },
        },
    },
    preview: {
        port: 4173,
        strictPort: true,
    },
});
