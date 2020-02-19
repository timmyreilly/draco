# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Catalog.Api/Catalog.Api.csproj", "Catalog.Api/"]
COPY ["Core.Hosting/Core.Hosting.csproj", "Core.Hosting/"]
COPY ["Core.Models/Core.Models.csproj", "Core.Models/"]
COPY ["Azure.Catalog/Azure.Catalog.csproj", "Azure.Catalog/"]
COPY ["Azure/Azure.csproj", "Azure/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Core.Catalog/Core.Catalog.csproj", "Core.Catalog/"]
COPY ["Models.Cosmos/Azure.Models.csproj", "Models.Cosmos/"]
RUN dotnet restore "Catalog.Api/Catalog.Api.csproj"
COPY . .
WORKDIR "/src/Catalog.Api"
RUN dotnet build "Catalog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]