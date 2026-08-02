#!/usr/bin/env bash

set -Eeuo pipefail

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
api_pid=""
frontend_pid=""

cleanup() {
  trap - EXIT INT TERM
  [[ -z "$frontend_pid" ]] || kill "$frontend_pid" 2>/dev/null || true
  [[ -z "$api_pid" ]] || kill "$api_pid" 2>/dev/null || true
  wait 2>/dev/null || true
}

trap cleanup EXIT INT TERM

if [[ ! -f "$root_dir/.env" ]]; then
  echo "Missing .env. Copy .env.example to .env and add the local values first." >&2
  exit 1
fi

dotnet run \
  --project "$root_dir/src/Coding.Api/Coding.Api.csproj" \
  --launch-profile http \
  --no-restore &
api_pid=$!

(
  cd "$root_dir/frontend"
  npm run dev
) &
frontend_pid=$!

wait -n "$api_pid" "$frontend_pid"
