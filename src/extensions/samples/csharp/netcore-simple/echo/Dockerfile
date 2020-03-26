# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "echo.csproj"
RUN dotnet build "echo.csproj" -c Release
FROM build AS publish
RUN dotnet publish "echo.csproj" -c Release 

ENTRYPOINT ["dotnet", "echo.dll"]
