# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/CRM.SocialDepartment.Site.csproj ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/
COPY ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Application/CRM.SocialDepartment.Application.csproj ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Application/
COPY ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Domain/CRM.SocialDepartment.Domain.csproj ./src/CRM.SocialDepartment.Core/CRM.SocialDepartment.Domain/
COPY ./src/CRM.SocialDepartment.Infrastructure/Implemententions/DataAccess/MongoDb/CRM.SocialDepartment.Infrastructure.DataAccess.MongoDb.csproj ./src/CRM.SocialDepartment.Infrastructure/Implemententions/DataAccess/MongoDb/
COPY ./src/CRM.SocialDepartment.Infrastructure/Interfaces/CRM.SocialDepartment.Infrastructure.Interfaces/CRM.SocialDepartment.Infrastructure.Interfaces.csproj ./src/CRM.SocialDepartment.Infrastructure/Interfaces/CRM.SocialDepartment.Infrastructure.Interfaces/
COPY ./src/Library/DDD/DDD.csproj ./src/Library/DDD/
# Если есть Identity-слой как отдельный проект, добавьте его при необходимости:
# COPY ./Identity/CRM.SocialDepartment.Infrastructure.Identity.csproj ./Identity/

# Восстановление
RUN dotnet restore ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/CRM.SocialDepartment.Site.csproj

COPY . .

# Публикация
RUN dotnet publish ./src/CRM.SocialDepartment.Presentation/CRM.SocialDepartment.Site/CRM.SocialDepartment.Site.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Kestrel внутри контейнера
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_ENVIRONMENT=Docker

# Приложение читает MongoDbSetting из конфигурации;
# ниже переопределим через переменные окружения в docker-compose.

EXPOSE 8080
ENTRYPOINT ["dotnet", "CRM.SocialDepartment.Site.dll"]
