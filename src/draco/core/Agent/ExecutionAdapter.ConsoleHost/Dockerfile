FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["core/Agent/ExecutionAdapter.ConsoleHost/ExecutionAdapter.ConsoleHost.csproj", "core/Agent/ExecutionAdapter.ConsoleHost/"]
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
RUN dotnet restore "core/Agent/ExecutionAdapter.ConsoleHost/ExecutionAdapter.ConsoleHost.csproj"
COPY . .
WORKDIR "/src/core/Agent/ExecutionAdapter.ConsoleHost"
RUN dotnet build "ExecutionAdapter.ConsoleHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExecutionAdapter.ConsoleHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExecutionAdapter.ConsoleHost.dll"]