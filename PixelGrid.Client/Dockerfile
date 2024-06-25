FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PixelGrid.Client/PixelGrid.Client.csproj", "PixelGrid.Client/"]
RUN dotnet restore "PixelGrid.Client/PixelGrid.Client.csproj"
COPY . .
WORKDIR "/src/PixelGrid.Client"
RUN dotnet build "PixelGrid.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PixelGrid.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelGrid.Client.dll"]
