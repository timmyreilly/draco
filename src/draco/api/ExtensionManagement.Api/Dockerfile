# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["api/ExtensionManagement.Api/ExtensionManagement.Api.csproj", "api/ExtensionManagement.Api/"]
COPY ["core/Core.Hosting/Core.Hosting.csproj", "core/Core.Hosting/"]
COPY ["core/Core.Models/Core.Models.csproj", "core/Core.Models/"]
COPY ["core/Core/Core.csproj", "core/Core/"]
COPY ["platforms/Azure/Models.Cosmos/Azure.Models.csproj", "platforms/Azure/Models.Cosmos/"]
COPY ["platforms/Azure/Azure/Azure.csproj", "platforms/Azure/Azure/"]
RUN dotnet restore "api/ExtensionManagement.Api/ExtensionManagement.Api.csproj"
COPY . .
WORKDIR "/src/api/ExtensionManagement.Api"
RUN dotnet build "ExtensionManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExtensionManagement.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExtensionManagement.Api.dll"]