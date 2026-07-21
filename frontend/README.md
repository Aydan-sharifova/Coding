# Frontend

React frontend for the Coding platform. Authentication is connected to the ASP.NET Core API through an HTTP-only refresh-token cookie and in-memory JWT access tokens.

## Local development

Start the API on its configured HTTP port (`5192`), then run:

```bash
npm install
npm run dev
```

Vite proxies `/api` to the backend, matching the same-origin setup used by Nginx in Docker. Set `VITE_API_URL` only when the API must be reached through a different origin.
