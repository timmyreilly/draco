# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["api/Execution.Api/Execution.Api.csproj", "api/Execution.Api/"]
COPY ["core/Core.Hosting/Core.Hosting.csproj", "core/Core.Hosting/"]
COPY ["platforms/Azure/Azure.ObjectStorage/Azure.ObjectStorage.csproj", "platforms/Azure/Azure.ObjectStorage/"]
COPY ["core/ObjectStorage/Core.ObjectStorage.csproj", "core/ObjectStorage/"]
COPY ["core/Core.Models/Core.Models.csproj", "core/Core.Models/"]
COPY ["core/Services/Core.Services.csproj", "core/Services/"]
COPY ["core/Core/Core.csproj", "core/Core/"]
COPY ["platforms/Azure/Azure/Azure.csproj", "platforms/Azure/Azure/"]
COPY ["platforms/Azure/Azure.Execution/Azure.Execution.csproj", "platforms/Azure/Azure.Execution/"]
COPY ["core/Core.Execution/Core.Execution.csproj", "core/Core.Execution/"]
COPY ["tests/IntegrationTests.HowdyService/IntegrationTests.HowdyService.csproj", "tests/IntegrationTests.HowdyService/"]
COPY ["platforms/Azure/Models.Cosmos/Azure.Models.csproj", "platforms/Azure/Models.Cosmos/"]
RUN dotnet restore "api/Execution.Api/Execution.Api.csproj"
COPY . .
WORKDIR "/src/api/Execution.Api"
RUN dotnet build "Execution.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Execution.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Execution.Api.dll"]