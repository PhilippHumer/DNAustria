FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Discover.DNAustria.Api/Discover.DNAustria.Api.csproj", "Discover.DNAustria.Api/"]
COPY ["Discover.DNAustria.Application/Discover.DNAustria.Application.csproj", "Discover.DNAustria.Application/"]
COPY ["Discover.DNAustria.Domain/Discover.DNAustria.Domain.csproj", "Discover.DNAustria.Domain/"]
COPY ["Discover.DNAustria.Infrastructure/Discover.DNAustria.Infrastructure.csproj", "Discover.DNAustria.Infrastructure/"]
RUN dotnet restore "Discover.DNAustria.Api/Discover.DNAustria.Api.csproj"
COPY . .
WORKDIR "/src/Discover.DNAustria.Api"
RUN dotnet publish "Discover.DNAustria.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Discover.DNAustria.Api.dll"]

