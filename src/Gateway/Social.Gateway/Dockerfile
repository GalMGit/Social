FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["src/Gateway/Social.Gateway/Social.Gateway.csproj", "src/Gateway/Social.Gateway/"]
COPY ["src/BuildingBlocks/Social.Shared/Social.Shared.csproj", "src/BuildingBlocks/Social.Shared/"]

RUN dotnet restore "src/Gateway/Social.Gateway/Social.Gateway.csproj"

COPY . .

WORKDIR "/src/src/Gateway/Social.Gateway"
RUN dotnet build "./Social.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Social.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

# Ocelot должен иметь возможность создавать/изменять конфигурацию
RUN mkdir -p /app/ocelot-configuration/production \
    && chown -R $APP_UID:$APP_UID /app/ocelot-configuration

USER $APP_UID

ENTRYPOINT ["dotnet", "Social.Gateway.dll"]
