#!/bin/bash

# Check if .env file exists
if [ ! -f .env ]; then
    echo "Copying .env.example to .env..."
    cp .env.example .env
    echo
    echo "WARNING: Please configure your .env file before running this script!"
    echo
    exit 1
fi

# Load environment variables
set -a
source .env
set +a

# Trim whitespace from variables
DOCKER_USER=$(echo "$DOCKER_USER" | xargs)
COMPOSE_PROJECT_NAME=$(echo "$COMPOSE_PROJECT_NAME" | xargs)
USER_ID=$(echo "$USER_ID" | xargs)
GROUP_ID=$(echo "$GROUP_ID" | xargs)
USERNAME=$(echo "$USERNAME" | xargs)
GROUPNAME=$(echo "$GROUPNAME" | xargs)
PROJECT_NAME=$(echo "$PROJECT_NAME" | xargs)
API_PORT=$(echo "$API_PORT" | xargs)
DB_HOST=$(echo "$DB_HOST" | xargs)
DB_NAME=$(echo "$DB_NAME" | xargs)
DB_USER=$(echo "$DB_USER" | xargs)
DB_PASSWORD=$(echo "$DB_PASSWORD" | xargs)
DB_PORT=$(echo "$DB_PORT" | xargs)

# Construct IMAGE_NAME
IMAGE_NAME="${DOCKER_USER}/${COMPOSE_PROJECT_NAME}-web-api"

# Copy Dockerfile from .docker/aspnet to root
cp ".docker/aspnet/Dockerfile" Dockerfile

# Configure the Dockerfile and replace ${PROJECT_NAME}
sed -i "s/\${PROJECT_NAME}/${PROJECT_NAME}/g" Dockerfile

# Execute docker build with environment variables
docker build --no-cache \
    --build-arg USER_ID="${USER_ID}" \
    --build-arg GROUP_ID="${GROUP_ID}" \
    --build-arg USERNAME="${USERNAME}" \
    --build-arg GROUPNAME="${GROUPNAME}" \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg PROJECT_NAME="${PROJECT_NAME}" \
    -t "${IMAGE_NAME}:latest" .

# Push the image
docker push "${IMAGE_NAME}:latest"

# Copy .env to deployment directory
cp .env .deployment/.env

echo "Build completed successfully!"