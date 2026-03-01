#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${1:-${API_BASE_URL:-http://localhost:5082}}"
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@ndis.local}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Admin123$}"
PROVIDER_PASSWORD="${PROVIDER_PASSWORD:-Provider123$}"

TMP_ROOT="${TMPDIR:-/tmp}"
WORK_DIR="$(mktemp -d "$TMP_ROOT/ndis-smoke.XXXXXX")"
RESPONSE_FILE=""
STATUS_CODE=""

trap 'rm -rf "$WORK_DIR"' EXIT

log() {
  printf '%s\n' "$*"
}

fail() {
  printf 'ERROR: %s\n' "$1" >&2

  if [ -n "$RESPONSE_FILE" ] && [ -s "$RESPONSE_FILE" ]; then
    printf 'Last response: %s\n' "$(tr -d '\n\r' < "$RESPONSE_FILE")" >&2
  fi

  exit 1
}

send_request() {
  local method="$1"
  local path="$2"
  local body="${3-}"
  local token="${4-}"
  local -a curl_args

  RESPONSE_FILE="$(mktemp "$WORK_DIR/response.XXXXXX")"
  curl_args=(
    -k
    -L
    -sS
    -o "$RESPONSE_FILE"
    -w "%{http_code}"
    -X "$method"
    -H "Accept: application/json"
  )

  if [ -n "$token" ]; then
    curl_args+=(-H "Authorization: Bearer $token")
  fi

  if [ -n "$body" ]; then
    curl_args+=(-H "Content-Type: application/json" -d "$body")
  fi

  curl_args+=("$BASE_URL$path")
  STATUS_CODE="$(curl "${curl_args[@]}")" || fail "Request failed: $method $path"
}

expect_status() {
  local expected="$1"
  local description="$2"

  if [ "$STATUS_CODE" != "$expected" ]; then
    fail "$description returned HTTP $STATUS_CODE (expected $expected)"
  fi
}

json_string() {
  local key="$1"

  tr -d '\n\r' < "$RESPONSE_FILE" \
    | sed -n "s/.*\"$key\":\"\\([^\"]*\\)\".*/\\1/p" \
    | head -n 1
}

assert_json_value() {
  local key="$1"
  local expected="$2"
  local actual

  actual="$(json_string "$key")"

  if [ "$actual" != "$expected" ]; then
    fail "Expected response field '$key' to be '$expected', got '$actual'"
  fi
}

log "Running NDIS API smoke test against $BASE_URL"

send_request "GET" "/api/participants"
expect_status "200" "Participant list"

RUN_ID="$(date +%s)"
PROVIDER_EMAIL="smoke+$RUN_ID@ndis.local"
PARTICIPANT_EMAIL="participant+$RUN_ID@ndis.local"
SCHEDULED_START="2026-03-01T09:00:00Z"
ACTUAL_START="2026-03-01T09:05:00Z"

REGISTER_BODY="$(printf '{"email":"%s","password":"%s"}' "$PROVIDER_EMAIL" "$PROVIDER_PASSWORD")"
send_request "POST" "/api/auth/register" "$REGISTER_BODY"
expect_status "200" "Provider registration"

LOGIN_PROVIDER_BODY="$(printf '{"email":"%s","password":"%s"}' "$PROVIDER_EMAIL" "$PROVIDER_PASSWORD")"
send_request "POST" "/api/auth/login" "$LOGIN_PROVIDER_BODY"
expect_status "200" "Provider login"
PROVIDER_TOKEN="$(json_string "token")"
[ -n "$PROVIDER_TOKEN" ] || fail "Provider login did not return a token"

LOGIN_ADMIN_BODY="$(printf '{"email":"%s","password":"%s"}' "$ADMIN_EMAIL" "$ADMIN_PASSWORD")"
send_request "POST" "/api/auth/login" "$LOGIN_ADMIN_BODY"
expect_status "200" "Admin login"
ADMIN_TOKEN="$(json_string "token")"
[ -n "$ADMIN_TOKEN" ] || fail "Admin login did not return a token"

PARTICIPANT_BODY="$(printf '{"fullName":"Smoke Test %s","ndisNumber":"NDIS-%s","email":"%s","phone":"0400000000"}' "$RUN_ID" "$RUN_ID" "$PARTICIPANT_EMAIL")"
send_request "POST" "/api/participants" "$PARTICIPANT_BODY"
expect_status "201" "Participant creation"
PARTICIPANT_ID="$(json_string "id")"
[ -n "$PARTICIPANT_ID" ] || fail "Participant creation did not return an id"

PROVIDER_BODY="$(printf '{"name":"Smoke Provider %s","abn":"ABN-%s","contactEmail":"%s","contactPhone":"0400000001"}' "$RUN_ID" "$RUN_ID" "$PROVIDER_EMAIL")"
send_request "POST" "/api/providers" "$PROVIDER_BODY"
expect_status "201" "Provider record creation"
PROVIDER_ID="$(json_string "id")"
[ -n "$PROVIDER_ID" ] || fail "Provider record creation did not return an id"

BOOKING_BODY="$(printf '{"participantId":"%s","providerId":"%s","scheduledStartUtc":"%s","durationMinutes":60,"serviceType":"Support Coordination","notes":"Smoke test booking"}' "$PARTICIPANT_ID" "$PROVIDER_ID" "$SCHEDULED_START")"
send_request "POST" "/api/bookings" "$BOOKING_BODY"
expect_status "201" "Booking creation"
BOOKING_ID="$(json_string "id")"
[ -n "$BOOKING_ID" ] || fail "Booking creation did not return an id"

send_request "POST" "/api/bookings/$BOOKING_ID/confirm"
expect_status "200" "Booking confirmation"
assert_json_value "status" "Confirmed"

DELIVERY_BODY="$(printf '{"bookingId":"%s","actualStartUtc":"%s","actualDurationMinutes":60,"notes":"Smoke test delivery"}' "$BOOKING_ID" "$ACTUAL_START")"
send_request "POST" "/api/servicedeliveries" "$DELIVERY_BODY" "$PROVIDER_TOKEN"
expect_status "201" "Service delivery creation"
DELIVERY_ID="$(json_string "id")"
[ -n "$DELIVERY_ID" ] || fail "Service delivery creation did not return an id"

send_request "POST" "/api/servicedeliveries/$DELIVERY_ID/submit" "" "$PROVIDER_TOKEN"
expect_status "200" "Service delivery submission"
assert_json_value "status" "Submitted"

send_request "POST" "/api/servicedeliveries/$DELIVERY_ID/approve" "" "$ADMIN_TOKEN"
expect_status "200" "Service delivery approval"
assert_json_value "status" "Approved"

CLAIM_BODY="$(printf '{"serviceDeliveryId":"%s","amount":123.45}' "$DELIVERY_ID")"
send_request "POST" "/api/claims" "$CLAIM_BODY"
expect_status "200" "Claim creation"
assert_json_value "status" "Draft"

log "Smoke test passed."
log "Created participant: $PARTICIPANT_ID"
log "Created provider: $PROVIDER_ID"
log "Created booking: $BOOKING_ID"
log "Created delivery: $DELIVERY_ID"
