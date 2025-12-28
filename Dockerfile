
# Base para runtime ASP.NET Core (inclui Microsoft.AspNetCore.App)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Etapa de build (SDK 9.0)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o csproj e restaura
COPY ["Portfolio.Apresentation/Portfolio.Apresentation.csproj", "Portfolio.Apresentation/"]
RUN dotnet restore "./Portfolio.Apresentation/Portfolio.Apresentation.csproj"

# Copia o restante do código
COPY . .
WORKDIR "/src/Portfolio.Apresentation"
RUN dotnet build "./Portfolio.Apresentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Portfolio.Apresentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final (runtime ASP.NET Core)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Portfolio.Apresentation.dll"]