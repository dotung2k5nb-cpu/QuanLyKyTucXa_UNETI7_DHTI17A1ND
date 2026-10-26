#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname -- "$0")"

if fuser -s 5080/tcp; then
  echo "Stopping the process using port 5080..."
  fuser -k -TERM 5080/tcp >/dev/null 2>&1 || true

  attempts=0
  while fuser -s 5080/tcp; do
    if (( attempts >= 5 )); then
      echo "Port 5080 is still in use; refusing to start another instance." >&2
      exit 1
    fi
    sleep 1
    ((attempts += 1))
  done
fi

dotnet run --urls http://localhost:5080
