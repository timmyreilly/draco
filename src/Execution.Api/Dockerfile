# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Execution.Api/Execution.Api.csproj", "Execution.Api/"]
COPY ["ObjectStorage/Core.ObjectStorage.csproj", "ObjectStorage/"]
COPY ["Core.Models/Core.Models.csproj", "Core.Models/"]
COPY ["Services/Core.Services.csproj", "Services/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Core.Hosting/Core.Hosting.csproj", "Core.Hosting/"]
COPY ["IntegrationTests.HowdyService/IntegrationTests.HowdyService.csproj", "IntegrationTests.HowdyService/"]
COPY ["Api.Modules/Api.Modules.csproj", "Api.Modules/"]
COPY ["Core.Execution/Core.Execution.csproj", "Core.Execution/"]
COPY ["Azure.ObjectStorage/Azure.ObjectStorage.csproj", "Azure.ObjectStorage/"]
COPY ["Azure/Azure.csproj", "Azure/"]
COPY ["Azure.Execution/Azure.Execution.csproj", "Azure.Execution/"]
COPY ["Models.Cosmos/Azure.Models.csproj", "Models.Cosmos/"]
RUN dotnet restore "Execution.Api/Execution.Api.csproj"
COPY . .
WORKDIR "/src/Execution.Api"
RUN dotnet build "Execution.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Execution.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Execution.Api.dll"]