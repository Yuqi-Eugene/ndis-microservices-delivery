#!/usr/bin/env bash
set -euo pipefail

# Run API from repo root regardless of current working directory
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root_dir="$(cd "$script_dir/.." && pwd)"

api_dir="$root_dir/src/Api"

if [ ! -d "$api_dir" ]; then
  echo "Error: cannot find API directory at: $api_dir" >&2
  echo "Current dir: $(pwd)" >&2
  exit 1
fi

cd "$root_dir"
dotnet run --project "$api_dir"