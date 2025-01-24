# Setup.ps1 - PowerShell script for Docker Compose management

# Variables
$DockerCompose = "docker-compose"

# Function to set up Docker containers
function Setup {
    Write-Host "Setting up Docker containers..."
    Invoke-Expression "$DockerCompose up --force-recreate -d"
}

# Function to clean up Docker containers and volumes
function Clean {
    Write-Host "Cleaning up Docker containers and volumes..."
    Invoke-Expression "$DockerCompose down -v"
}

# Function to run database migrations in the API container
function Migrate {
    Write-Host "Running database migrations..."
    Invoke-Expression "$DockerCompose exec api dotnet ef database update"
}

# Function to display help
function Help {
    Write-Host "Available commands:"
    Write-Host "  .\Setup.ps1 Setup   - Compose the Docker containers"
    Write-Host "  .\Setup.ps1 Clean   - Delete the containers and volumes"
    Write-Host "  .\Setup.ps1 Migrate - Run 'dotnet ef database update' in the API container"
    Write-Host "  .\Setup.ps1 Help    - Display this help message"
}

# Main script logic
if ($args.Count -eq 0) {
    Help
    exit
}

switch ($args[0].ToLower()) {
    "setup" {
        Setup
    }
    "clean" {
        Clean
    }
    "migrate" {
        Migrate
    }
    "help" {
        Help
    }
    default {
        Write-Host "Unknown command: $($args[0])"
        Help
    }
}