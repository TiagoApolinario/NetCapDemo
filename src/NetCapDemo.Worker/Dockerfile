FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NetCapDemo.Worker/NetCapDemo.Worker.csproj", "src/NetCapDemo.Worker/"]
RUN dotnet restore "src/NetCapDemo.Worker/NetCapDemo.Worker.csproj"
COPY . .
WORKDIR "/src/src/NetCapDemo.Worker"
RUN dotnet build "NetCapDemo.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NetCapDemo.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetCapDemo.Worker.dll"]
