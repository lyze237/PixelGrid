FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PixelGrid.Web/Server/PixelGrid.Web.Server.csproj", "PixelGrid.Web/Server/"]
COPY ["PixelGrid.Web/Client/PixelGrid.Web.Client.csproj", "PixelGrid.Web/Client/"]
COPY ["PixelGrid.Web/Shared/PixelGrid.Web.Shared.csproj", "PixelGrid.Web/Shared/"]
RUN dotnet restore "PixelGrid.Web/Server/PixelGrid.Web.Server.csproj"
COPY . .
WORKDIR "/src/PixelGrid.Web/Server"
RUN dotnet build "PixelGrid.Web.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PixelGrid.Web.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelGrid.Web.Server.dll"]
