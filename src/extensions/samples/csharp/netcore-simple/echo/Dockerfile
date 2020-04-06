# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Get runtime for aspnet core
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Get SDK so we can build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# csproj
COPY *.csproj ./
RUN dotnet restore "echo.csproj"

# Copy everything else
COPY . ./

# Build
RUN dotnet build "echo.csproj" -c Release -o /app/build

# Buildruntime
FROM build AS publish
RUN dotnet publish "echo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "echo.dll"]
