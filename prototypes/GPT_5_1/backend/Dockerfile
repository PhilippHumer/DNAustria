# Backend API Dockerfile (Multi-Stage)
# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy nur benötigte Projektdateien
COPY backend/Domain/Domain.csproj backend/Domain/Domain.csproj
COPY backend/Application/Application.csproj backend/Application/Application.csproj
COPY backend/Infrastructure/Infrastructure.csproj backend/Infrastructure/Infrastructure.csproj
COPY backend/API/API.csproj backend/API/API.csproj

# Restore nur API (zieht Referenzen automatisch nach)
RUN dotnet restore backend/API/API.csproj

# Quellcode kopieren
COPY backend/ backend/

RUN dotnet publish backend/API/API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:5100 \
    ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5100
ENTRYPOINT ["dotnet", "API.dll"]
