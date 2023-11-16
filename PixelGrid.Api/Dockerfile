FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PixelGrid.Api/PixelGrid.Api.csproj", "PixelGrid.Api/"]
RUN dotnet restore "PixelGrid.Api/PixelGrid.Api.csproj"
COPY . .
WORKDIR "/src/PixelGrid.Api"
RUN dotnet build "PixelGrid.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PixelGrid.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelGrid.Api.dll"]
