#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["ExtensionManagement.Api/ExtensionManagement.Api.csproj", "ExtensionManagement.Api/"]
COPY ["Core.Hosting/Core.Hosting.csproj", "Core.Hosting/"]
COPY ["Core.Models/Core.Models.csproj", "Core.Models/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Models.Cosmos/Azure.Models.csproj", "Models.Cosmos/"]
COPY ["Azure/Azure.csproj", "Azure/"]
RUN dotnet restore "ExtensionManagement.Api/ExtensionManagement.Api.csproj"
COPY . .
WORKDIR "/src/ExtensionManagement.Api"
RUN dotnet build "ExtensionManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExtensionManagement.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExtensionManagement.Api.dll"]