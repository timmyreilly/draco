# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Get runtime for aspnet core
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Get SDK so we can build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# csproj
COPY *.csproj ./
RUN dotnet restore

# Copy everything else into the docker working dir
COPY . ./

# Build
RUN dotnet build "echo.csproj" -c Release -o /app/build

# Build runtime using the full SDK
FROM build AS publish
RUN dotnet publish "echo.csproj" -c Release -o /app/publish

# switch to the runtime only
FROM base AS final
WORKDIR /app

# Copy only the published part of the app
COPY --from=publish /app/publish .

#set entry point
ENTRYPOINT ["dotnet", "echo.dll"]
