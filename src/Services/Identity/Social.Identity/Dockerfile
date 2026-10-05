FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Identity/Social.Identity/Social.Identity.csproj", "src/Services/Identity/Social.Identity/"]
COPY ["src/BuildingBlocks/Social.Contracts/Social.Contracts.csproj", "src/BuildingBlocks/Social.Contracts/"]
COPY ["src/BuildingBlocks/Social.Shared/Social.Shared.csproj", "src/BuildingBlocks/Social.Shared/"]
RUN dotnet restore "src/Services/Identity/Social.Identity/Social.Identity.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/Social.Identity"
RUN dotnet build "./Social.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Social.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Social.Identity.dll"]
