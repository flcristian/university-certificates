# Makefile for Docker Compose management

# Variables
DOCKER_COMPOSE := docker-compose
API_CONTAINER := ${COMPOSE_PROJECT_NAME:-app}-api
DB_CONTAINER := ${COMPOSE_PROJECT_NAME:-app}-mariadb

# Default target
all: setup

# Compose the Docker containers
setup:
	@echo "Setting up Docker containers..."
	$(DOCKER_COMPOSE) up --force-recreate -d

# Delete the containers and volumes
clean:
	@echo "Cleaning up Docker containers and volumes..."
	$(DOCKER_COMPOSE) down -v

# Run database migrations in the API container
migrate:
	@echo "Running database migrations..."
	$(DOCKER_COMPOSE) exec api dotnet ef database update

# Help target to display available commands
help:
	@echo "Available commands:"
	@echo "  make setup   - Compose the Docker containers"
	@echo "  make clean   - Delete the containers and volumes"
	@echo "  make migrate - Run 'dotnet ef database update' in the API container"

.PHONY: all setup clean migrate help