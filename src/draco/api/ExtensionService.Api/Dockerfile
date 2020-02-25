# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["api/ExtensionService.Api/ExtensionService.Api.csproj", "api/ExtensionService.Api/"]
RUN dotnet restore "api/ExtensionService.Api/ExtensionService.Api.csproj"
COPY . .
WORKDIR "/src/api/ExtensionService.Api"
RUN dotnet build "ExtensionService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExtensionService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExtensionService.Api.dll"]