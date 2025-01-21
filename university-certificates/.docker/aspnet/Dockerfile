# Dockerfile
# Get global arguments
ARG USERNAME=dotnet
ARG GROUPNAME=dotnet
ARG USER_ID=1000
ARG GROUP_ID=1000
ARG ENVIRONMENT
ARG BUILD_DATE
ARG REVISION

# Base image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS builder

ARG BUILD_DATE
ARG REVISION

# Labels
LABEL maintainer="flcristian (qflorescucristian@gmail.com)"
LABEL org.opencontainers.image.created=${BUILD_DATE}
LABEL org.opencontainers.image.revision=${REVISION}
LABEL org.opencontainers.image.title=".NET Development Image"
LABEL org.opencontainers.image.description="Development-friendly .NET Docker image"

# Set local arguments
ARG USERNAME
ARG GROUPNAME
ARG USER_ID
ARG GROUP_ID
ARG ENVIRONMENT

# Set environment variables
ENV USERNAME=${USERNAME:-dotnet}
ENV GROUPNAME=${GROUPNAME:-dotnet}
ENV USER_ID=${USER_ID:-1000}
ENV GROUP_ID=${GROUP_ID:-1000}
ENV ENVIRONMENT=${ENVIRONMENT:-production}
ENV ASPNETCORE_ENVIRONMENT=${ENVIRONMENT:-production}
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV NUGET_XMLDOC_MODE=skip
ENV NUGET_PACKAGES=/home/${USERNAME}/.nuget/packages

# Install required packages and create user
RUN apk add --no-cache curl=8.5.0-r0 && \
    addgroup -g ${GROUP_ID} ${GROUPNAME} && \
    adduser -D -u ${USER_ID} -G ${GROUPNAME} ${USERNAME}

# Create necessary directories and set permissions
RUN mkdir -p /app /app/tmp /home/${USERNAME}/.nuget/packages && \
    chown -R ${USERNAME}:${GROUPNAME} /app /home/${USERNAME}/.nuget

# Set working directory
WORKDIR /app

# Install development tools for non-production environments
RUN if [ "$ENVIRONMENT" != "production" ]; then \
    dotnet tool install --global dotnet-watch; \
    fi

# Add dotnet tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Create volumes for development
VOLUME ["/app", "/home/${USERNAME}/.nuget/packages"]

# Expose ports
EXPOSE 80

# Healthcheck
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

# Use different commands for development and production
CMD ["sh", "-c", "if [ \"$ENVIRONMENT\" != \"production\" ]; then dotnet watch run --no-launch-profile; else dotnet run --no-launch-profile; fi"]