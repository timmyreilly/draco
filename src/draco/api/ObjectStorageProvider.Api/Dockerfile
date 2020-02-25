# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["api/ObjectStorageProvider.Api/ObjectStorageProvider.Api.csproj", "api/ObjectStorageProvider.Api/"]
COPY ["core/Core.Hosting/Core.Hosting.csproj", "core/Core.Hosting/"]
COPY ["api/Api.Modules/Api.Modules.csproj", "api/Api.Modules/"]
COPY ["core/Core.Models/Core.Models.csproj", "core/Core.Models/"]
COPY ["tests/IntegrationTests.HowdyService/IntegrationTests.HowdyService.csproj", "tests/IntegrationTests.HowdyService/"]
COPY ["core/Services/Core.Services.csproj", "core/Services/"]
COPY ["core/Core/Core.csproj", "core/Core/"]
COPY ["core/Core.Execution/Core.Execution.csproj", "core/Core.Execution/"]
COPY ["core/ObjectStorage/Core.ObjectStorage.csproj", "core/ObjectStorage/"]
COPY ["platforms/Azure/Azure.ObjectStorage/Azure.ObjectStorage.csproj", "platforms/Azure/Azure.ObjectStorage/"]
COPY ["platforms/Azure/Azure/Azure.csproj", "platforms/Azure/Azure/"]
COPY ["api/Api.InternalModels/Api.InternalModels.csproj", "api/Api.InternalModels/"]
RUN dotnet restore "api/ObjectStorageProvider.Api/ObjectStorageProvider.Api.csproj"
COPY . .
WORKDIR "/src/api/ObjectStorageProvider.Api"
RUN dotnet build "ObjectStorageProvider.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ObjectStorageProvider.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ObjectStorageProvider.Api.dll"]