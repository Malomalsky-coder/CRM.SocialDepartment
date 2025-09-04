# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

COPY ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/CRM.SocialDepartment.Site.csproj ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/
COPY ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Application/CRM.SocialDepartment.Application.csproj ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Application/
COPY ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Domain/CRM.SocialDepartment.Domain.csproj ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Domain/
COPY ./src/CRM.SocialDepartment.Infrastructure/Implemententions/DataAccess/MongoDb/CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.csproj ./src/CRM.SocialDepartment.Infrastructure/Implemententions/DataAccess/MongoDb/
COPY ./src/CRM.SocialDepartment.Infrastructure/Interfaces/CRM.SocialDepartment.Infrastructure.Interfaces/CRM.SocialDepartment.Infrastructure.Interfaces.csproj ./src/CRM.SocialDepartment.Infrastructure/Interfaces/CRM.SocialDepartment.Infrastructure.Interfaces/
COPY ./src/Library/DDD/DDD.csproj ./src/Library/DDD/

COPY ./NuGet.config ./

RUN --mount=type=cache,id=nuget-packages,target=/root/.nuget/packages \
    --mount=type=cache,id=nuget-v3,target=/root/.local/share/NuGet/v3-cache \
    bash -lc '\
      for i in 1 2 3 4; do \
        dotnet restore ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/CRM.SocialDepartment.Site.csproj \
          --configfile ./NuGet.config --disable-parallel --nologo && exit 0; \
        echo "restore failed, retry $i/4"; sleep 10; \
      done; \
      exit 1 \
    '

COPY . .

RUN --mount=type=cache,id=nuget-packages,target=/root/.nuget/packages \
    --mount=type=cache,id=nuget-v3,target=/root/.local/share/NuGet/v3-cache \
    dotnet publish ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/CRM.SocialDepartment.Site.csproj \
      -c Release -o /app/publish /p:UseAppHost=false --nologo

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_ENVIRONMENT=Docker
EXPOSE 8080
ENTRYPOINT ["dotnet", "CRM.SocialDepartment.Site.dll"]
