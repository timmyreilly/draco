FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["ExecutionAdapter.ConsoleHost/ExecutionAdapter.ConsoleHost.csproj", "ExecutionAdapter.ConsoleHost/"]
COPY ["Core.Hosting/Core.Hosting.csproj", "Core.Hosting/"]
COPY ["Azure.ObjectStorage/Azure.ObjectStorage.csproj", "Azure.ObjectStorage/"]
COPY ["ObjectStorage/Core.ObjectStorage.csproj", "ObjectStorage/"]
COPY ["Core.Models/Core.Models.csproj", "Core.Models/"]
COPY ["Services/Core.Services.csproj", "Services/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Azure/Azure.csproj", "Azure/"]
COPY ["Azure.Execution/Azure.Execution.csproj", "Azure.Execution/"]
COPY ["Core.Execution/Core.Execution.csproj", "Core.Execution/"]
COPY ["IntegrationTests.HowdyService/IntegrationTests.HowdyService.csproj", "IntegrationTests.HowdyService/"]
RUN dotnet restore "ExecutionAdapter.ConsoleHost/ExecutionAdapter.ConsoleHost.csproj"
COPY . .
WORKDIR "/src/ExecutionAdapter.ConsoleHost"
RUN dotnet build "ExecutionAdapter.ConsoleHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExecutionAdapter.ConsoleHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExecutionAdapter.ConsoleHost.dll"]