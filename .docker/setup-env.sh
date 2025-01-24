#!/bin/bash

# Get current user and group IDs
USER_ID=$(id -u)
GROUP_ID=$(id -g)

# Path to .env file
ENV_FILE="../.env"

# Check if .env file exists
if [ ! -f "$ENV_FILE" ]; then
    echo "Error: .env file not found at $ENV_FILE"
    exit 1
fi

# Update or add USER_ID and GROUP_ID in .env file
if grep -q "^USER_ID=" "$ENV_FILE"; then
    sed -i "s/^USER_ID=.*/USER_ID=$USER_ID/" "$ENV_FILE"
else
    echo "USER_ID=$USER_ID" >> "$ENV_FILE"
fi

if grep -q "^GROUP_ID=" "$ENV_FILE"; then
    sed -i "s/^GROUP_ID=.*/GROUP_ID=$GROUP_ID/" "$ENV_FILE"
else
    echo "GROUP_ID=$GROUP_ID" >> "$ENV_FILE"
fi

echo "Updated .env file with USER_ID=$USER_ID and GROUP_ID=$GROUP_ID"