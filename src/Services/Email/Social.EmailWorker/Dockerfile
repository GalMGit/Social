FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/Services/Email/Social.EmailWorker/Social.EmailWorker.csproj", "src/Services/Email/Social.EmailWorker/"]
COPY ["src/BuildingBlocks/Social.Contracts/Social.Contracts.csproj", "src/BuildingBlocks/Social.Contracts/"]
RUN dotnet restore "src/Services/Email/Social.EmailWorker/Social.EmailWorker.csproj"
COPY . .
WORKDIR "/src/src/Services/Email/Social.EmailWorker"
RUN dotnet build "./Social.EmailWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Social.EmailWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Social.EmailWorker.dll"]
