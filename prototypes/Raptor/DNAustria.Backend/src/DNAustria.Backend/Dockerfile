FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ["src/DNAustria.Backend/DNAustria.Backend.csproj", "src/DNAustria.Backend/"]
RUN dotnet restore "src/DNAustria.Backend/DNAustria.Backend.csproj"

# copy everything and build
COPY . .
WORKDIR "/src/src/DNAustria.Backend"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Bind to these addresses inside container
ENV ASPNETCORE_URLS="http://+:5500;https://+:5501"
EXPOSE 5500 5501
ENTRYPOINT ["dotnet", "DNAustria.Backend.dll"]