FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Users/Users.Api/Users.Api.csproj", "src/Users/Users.Api/"]
COPY ["src/Users/Users.Application/Users.Application.csproj", "src/Users/Users.Application/"]
COPY ["src/Users/Users.Domain/Users.Domain.csproj", "src/Users/Users.Domain/"]
COPY ["src/Users/Users.Infrastructure/Users.Infrastructure.csproj", "src/Users/Users.Infrastructure/"]
RUN dotnet restore "src/Users/Users.Api/Users.Api.csproj"
COPY . .
WORKDIR "/src/src/Users/Users.Api"
RUN dotnet build "./Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Api.dll"]
