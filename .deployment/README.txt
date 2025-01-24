# ASP.NET Setup Template

This repository is a template for setting up an ASP.NET project with Docker deployment. It includes all the necessary configurations to streamline development and deployment.

## Features

- ASP.NET Core project ready for development.
- Docker support for containerized deployments.
- Environment configuration using `.env` file.
- Configurable API and database connection settings.

---

## Getting Started

### Deployment Prerequisites

- [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/install/) installed.

### Manual Server Deployment Setup Instructions

1. **Clone the repository**:

   ```bash
   git clone https://github.com/your-repo/aspnet-setup-template.git
   cd aspnet-setup-template
   ```

2. **Configure the `.env` file**:

   - Run the "run.bat"/"run.sh" script based on your operating system if you don't have the .env file yet.
   - Replace the placeholder values with your desired configuration. Example:

     ```env
     # Project Configuration
     DOCKER_USER=florescucristian
     COMPOSE_PROJECT_NAME=aspnet-setup-template

     # User Configuration (use your host user ID and group ID)
     USER_ID=1000
     GROUP_ID=1000

     # API Configuration
     API_PORT=8000

     # Database Connection
     DB_HOST=localhost
     DB_NAME=aspnet-setup-template
     DB_USER=aspnet
     DB_PASSWORD=admin
     DB_PORT=3308
     ```

3. **Build the app and set it up for deployment**:

   ```bash
   # Windows
   ./setup-deployment.bat
   
   # MacOS / Linux
   ./setup-deployment.sh
   ```

4. **Copy the .deployment folder contents to the directory/server where you want to compose the docker containers**
5. **Compose the docker containers**

    ```bash
    # Windows
    ./Setup.ps1 Setup

    # MacOS / Linux
    make setup
    ```

6. **Run the migrations**

    ```bash
    # Windows
    ./Setup.ps1 Migrate

    # MacOS / Linux
    make migrate
    ```

7. **Access the API**:

   The API will be available at `http://localhost:8000` (or the port configured in the `.env` file).

## Configuration Details

### .env File

The `.env` file contains key configurations for the project:

| Variable                | Description                                                   | Example Value          |
|-------------------------|---------------------------------------------------------------|------------------------|
| `DOCKER_USER`           | Docker user to own the containers.                            | `florescucristian`     |
| `COMPOSE_PROJECT_NAME`  | Name of the Docker Compose project.                           | `aspnet-setup-template`|
| `USER_ID`               | Host user ID for mapping in containers.                       | `1000`                |
| `GROUP_ID`              | Host group ID for mapping in containers.                      | `1000`                |
| `API_PORT`              | Port where the API will be exposed.                           | `8000`                |
| `DB_HOST`               | Host for the database connection.                             | `localhost`           |
| `DB_NAME`               | Name of the database.                                         | `aspnet-setup-template`|
| `DB_USER`               | Username for the database connection.                         | `aspnet`              |
| `DB_PASSWORD`           | Password for the database connection.                         | `admin`               |
| `DB_PORT`               | Port for the database connection.                             | `3308`                |

## Development Notes

- Ensure the `.env` file is not committed to version control by adding it to your `.gitignore` file.

## Troubleshooting

1. **Docker permissions error**:
   Ensure your user is added to the `docker` group:

   ```bash
   sudo usermod -aG docker $USER
   ```

2. **Database connection issues**:
   Confirm that the database configuration in the `.env` file matches your setup.

3. **Port conflicts**:
   If the API or database ports are already in use, update them in the `.env` file and restart the services.