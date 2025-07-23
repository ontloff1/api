# Базовый образ с Debian Bullseye
FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

# Установка SSL-поддержки
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    libnss3 \
    libssl1.1 \
 && update-ca-certificates \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

# SDK-слой
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FiasApi.csproj", "./"]
RUN dotnet restore "./FiasApi.csproj"
COPY . .
RUN dotnet build "FiasApi.csproj" -c Release -o /app/build

# Публикация
FROM build AS publish
RUN dotnet publish "FiasApi.csproj" -c Release -o /app/publish

# Финальный runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FiasApi.dll"]
