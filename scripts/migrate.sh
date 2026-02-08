#!/usr/bin/env bash
set -euo pipefail

# Usage: ./scripts/migrate.sh <MigrationName>
name="${1:-Migration}"

# Resolve repo root (one level up from /scripts), regardless of where the script is called from
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root_dir="$(cd "$script_dir/.." && pwd)"

api_csproj="$root_dir/src/Api/Api.csproj"

# Helpful error if paths are wrong
if [ ! -f "$api_csproj" ]; then
  echo "Error: cannot find Api.csproj at: $api_csproj" >&2
  echo "Current dir: $(pwd)" >&2
  exit 1
fi

dotnet ef migrations add "$name" --project "$api_csproj" --startup-project "$api_csproj"
dotnet ef database update --project "$api_csproj" --startup-project "$api_csproj"