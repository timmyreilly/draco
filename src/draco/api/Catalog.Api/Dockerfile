# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["api/Catalog.Api/Catalog.Api.csproj", "api/Catalog.Api/"]
COPY ["core/Core.Hosting/Core.Hosting.csproj", "core/Core.Hosting/"]
COPY ["core/Core.Models/Core.Models.csproj", "core/Core.Models/"]
COPY ["platforms/Azure/Azure.Catalog/Azure.Catalog.csproj", "platforms/Azure/Azure.Catalog/"]
COPY ["core/Core/Core.csproj", "core/Core/"]
COPY ["core/Core.Catalog/Core.Catalog.csproj", "core/Core.Catalog/"]
COPY ["platforms/Azure/Azure/Azure.csproj", "platforms/Azure/Azure/"]
COPY ["platforms/Azure/Models.Cosmos/Azure.Models.csproj", "platforms/Azure/Models.Cosmos/"]
RUN dotnet restore "api/Catalog.Api/Catalog.Api.csproj"
COPY . .
WORKDIR "/src/api/Catalog.Api"
RUN dotnet build "Catalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]