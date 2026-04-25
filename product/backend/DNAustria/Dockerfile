FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Packages.props ./
COPY global.json ./
COPY DNAustria.sln ./
COPY DNAustria.Api/DNAustria.Api.csproj DNAustria.Api/
COPY DNAustria.Dal/DNAustria.Dal.csproj DNAustria.Dal/
COPY DNAustria.Domain/DNAustria.Domain.csproj DNAustria.Domain/
COPY DNAustria.Logic/DNAustria.Logic.csproj DNAustria.Logic/

RUN dotnet restore DNAustria.sln

COPY . .
RUN dotnet publish DNAustria.Api/DNAustria.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libldap2 libsasl2-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

EXPOSE 5001

ENTRYPOINT ["dotnet", "DNAustria.Api.dll"]
