#!/bin/bash
set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
RESOURCE_GROUP="rg-"
LOCATION="eastus2"
ACR_NAME=""
ENVIRONMENT="-"
APP_NAME="nextdns-mcp"
IMAGE_TAG="latest"
IMAGE_NAME="${ACR_NAME}.azurecr.io/${APP_NAME}:${IMAGE_TAG}"

# ---------------------------------------------------------------------------
# Required secrets — set these before running or export them in your shell
# ---------------------------------------------------------------------------
: "${NEXTDNS_API_KEY:?NEXTDNS_API_KEY must be set}"
: "${NEXTDNS_PROFILE_ID:?NEXTDNS_PROFILE_ID must be set}"

# ---------------------------------------------------------------------------
# 1. Build and push image to ACR (remote build, no local Docker required)
# ---------------------------------------------------------------------------
echo "==> Building and pushing image to ACR..."
az acr build \
  --registry "$ACR_NAME" \
  --resource-group "General" \
  --image "${APP_NAME}:${IMAGE_TAG}" \
  .

# ---------------------------------------------------------------------------
# 2. Deploy (or update) the Container App in the existing environment
#    No ingress — this is a stdio-transport MCP server, not an HTTP service.
# ---------------------------------------------------------------------------
if az containerapp show \
     --name "$APP_NAME" \
     --resource-group "$RESOURCE_GROUP" \
     &>/dev/null; then

  echo "==> Updating existing Container App '${APP_NAME}'..."
  az containerapp update \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --image "$IMAGE_NAME" \
    --set-env-vars \
        "NEXTDNS_API_KEY=secretref:nextdns-api-key" \
        "NEXTDNS_PROFILE_ID=secretref:nextdns-profile-id"

else

  echo "==> Creating Container App '${APP_NAME}'..."
  az containerapp create \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --environment "$ENVIRONMENT" \
    --image "$IMAGE_NAME" \
    --registry-server "${ACR_NAME}.azurecr.io" \
    --registry-identity system \
    --cpu 0.25 \
    --memory 0.5Gi \
    --min-replicas 1 \
    --max-replicas 1 \
    --ingress internal \
    --target-port 8080 \
    --secrets \
        "nextdns-api-key=${NEXTDNS_API_KEY}" \
        "nextdns-profile-id=${NEXTDNS_PROFILE_ID}" \
    --env-vars \
        "NEXTDNS_API_KEY=secretref:nextdns-api-key" \
        "NEXTDNS_PROFILE_ID=secretref:nextdns-profile-id"

fi

echo "==> Done. Image: ${IMAGE_NAME}"
