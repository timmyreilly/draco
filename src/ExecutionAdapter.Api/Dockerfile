# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["ExecutionAdapter.Api/ExecutionAdapter.Api.csproj", "ExecutionAdapter.Api/"]
COPY ["Core.Hosting/Core.Hosting.csproj", "Core.Hosting/"]
COPY ["IntegrationTests.HowdyService/IntegrationTests.HowdyService.csproj", "IntegrationTests.HowdyService/"]
COPY ["Services/Core.Services.csproj", "Services/"]
COPY ["Core.Models/Core.Models.csproj", "Core.Models/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Api.Modules/Api.Modules.csproj", "Api.Modules/"]
COPY ["Core.Execution/Core.Execution.csproj", "Core.Execution/"]
COPY ["ObjectStorage/Core.ObjectStorage.csproj", "ObjectStorage/"]
COPY ["Azure.ObjectStorage/Azure.ObjectStorage.csproj", "Azure.ObjectStorage/"]
COPY ["Azure/Azure.csproj", "Azure/"]
COPY ["Api.InternalModels/Api.InternalModels.csproj", "Api.InternalModels/"]
COPY ["Azure.Execution/Azure.Execution.csproj", "Azure.Execution/"]
RUN dotnet restore "ExecutionAdapter.Api/ExecutionAdapter.Api.csproj"
COPY . .
WORKDIR "/src/ExecutionAdapter.Api"
RUN dotnet build "ExecutionAdapter.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExecutionAdapter.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExecutionAdapter.Api.dll"]