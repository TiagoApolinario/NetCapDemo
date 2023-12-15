FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NetCapDemo.Web/NetCapDemo.Web.csproj", "src/NetCapDemo.Web/"]
RUN dotnet restore "src/NetCapDemo.Web/NetCapDemo.Web.csproj"
COPY . .
WORKDIR "/src/src/NetCapDemo.Web"
RUN dotnet build "NetCapDemo.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NetCapDemo.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetCapDemo.Web.dll"]
