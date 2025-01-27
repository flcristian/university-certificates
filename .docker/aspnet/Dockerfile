FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG USER_ID=1000 
ARG GROUP_ID=1000 
ARG USERNAME=aspnet 
ARG GROUPNAME=aspnet

# Install GDI+ dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    && rm -rf /var/lib/apt/lists/*

# Set up user and group
RUN groupadd -g ${GROUP_ID} ${GROUPNAME} && \
    useradd -u ${USER_ID} -g ${GROUPNAME} -m ${USERNAME}

WORKDIR /app
EXPOSE 8080

# Ensure correct permissions
RUN chown -R ${USERNAME}:${GROUPNAME} /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG USER_ID=1000 
ARG GROUP_ID=1000 
ARG USERNAME=aspnet 
ARG GROUPNAME=aspnet

# Install build dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    && rm -rf /var/lib/apt/lists/*

# Set up user and group
RUN groupadd -g ${GROUP_ID} ${GROUPNAME} && \
    useradd -u ${USER_ID} -g ${GROUPNAME} -m ${USERNAME}

ENV BUILD_CONFIGURATION=Release
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_ENVIRONMENT=Production

WORKDIR /src
COPY . .

RUN dotnet restore "${PROJECT_NAME}.csproj" --verbosity detailed

WORKDIR /src
RUN dotnet build "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
ARG USER_ID=1000 
ARG GROUP_ID=1000 
ARG USERNAME=aspnet 
ARG GROUPNAME=aspnet

# Install runtime dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    && rm -rf /var/lib/apt/lists/*

# Set up user and group
RUN groupadd -g ${GROUP_ID} ${GROUPNAME} && \
    useradd -u ${USER_ID} -g ${GROUPNAME} -m ${USERNAME}

WORKDIR /app
COPY --from=publish /app/publish . 
COPY .env .

RUN mkdir -p /app && \
    chown -R ${USERNAME}:${GROUPNAME} /app

USER ${USER_ID}:${GROUP_ID}

# Set production environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "${PROJECT_NAME}.dll"]