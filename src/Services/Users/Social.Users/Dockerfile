FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Users/Social.Users/Social.Users.csproj", "src/Services/Users/Social.Users/"]
COPY ["src/BuildingBlocks/Social.Contracts/Social.Contracts.csproj", "src/BuildingBlocks/Social.Contracts/"]
COPY ["src/BuildingBlocks/Social.Shared/Social.Shared.csproj", "src/BuildingBlocks/Social.Shared/"]
RUN dotnet restore "src/Services/Users/Social.Users/Social.Users.csproj"
COPY . .
WORKDIR "/src/src/Services/Users/Social.Users"
RUN dotnet build "./Social.Users.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Social.Users.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Social.Users.dll"]
