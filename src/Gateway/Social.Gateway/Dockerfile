FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
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
ENTRYPOINT ["dotnet", "Social.Gateway.dll"]
