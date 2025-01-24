@echo off
REM Check if .env file exists
if not exist .env (
    echo Copying .env.example to .env...
    copy .env.example .env
    echo.
    echo WARNING: Please configure your .env file before running this script!
    echo.
    exit /b 1
)

REM Load environment variables from .env
for /f "tokens=*" %%a in (.env) do (
    set %%a
)

REM Trim whitespace from variables
set DOCKER_USER=%DOCKER_USER: =%
set COMPOSE_PROJECT_NAME=%COMPOSE_PROJECT_NAME: =%
set USER_ID=%USER_ID: =%
set GROUP_ID=%GROUP_ID: =%
set USERNAME=%USERNAME: =%
set GROUPNAME=%GROUPNAME: =%
set PROJECT_NAME=%PROJECT_NAME: =%
set API_PORT=%API_PORT: =%
set DB_HOST=%DB_HOST: =%
set DB_NAME=%DB_NAME: =%
set DB_USER=%DB_USER: =%
set DB_PASSWORD=%DB_PASSWORD: =%
set DB_PORT=%DB_PORT: =%

REM Construct IMAGE_NAME
set IMAGE_NAME=%DOCKER_USER%/%COMPOSE_PROJECT_NAME%-web-api

REM Copy Dockerfile from .docker/aspnet to root
copy ".docker\aspnet\Dockerfile" Dockerfile

REM Configure the Dockerfile and replace ${PROJECT_NAME}
setlocal enabledelayedexpansion
set "inputFile=Dockerfile"
set "outputFile=Dockerfile.tmp"

(for /f "delims=" %%a in (%inputFile%) do (
   set "line=%%a"
   set "line=!line:${PROJECT_NAME}=%PROJECT_NAME%!"
   echo !line!
)) > %outputFile%

move /y %outputFile% %inputFile%

REM Execute docker build with environment variables
docker build --no-cache ^
    --build-arg USER_ID=%USER_ID% ^
    --build-arg GROUP_ID=%GROUP_ID% ^
    --build-arg USERNAME=%USERNAME% ^
    --build-arg GROUPNAME=%GROUPNAME% ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg PROJECT_NAME=%PROJECT_NAME% ^
    -t "%IMAGE_NAME%:latest" .

docker push "%IMAGE_NAME%:latest"
copy .env .deployment\.env

echo Build completed successfully!