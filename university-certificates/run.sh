#!/bin/bash
docker compose down -v
dotnet restore
docker compose -f docker-compose.development.yaml up -d