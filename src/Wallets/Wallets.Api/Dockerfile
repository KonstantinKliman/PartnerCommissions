FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Wallets/Wallets.Api/Wallets.Api.csproj", "src/Wallets/Wallets.Api/"]
COPY ["src/Wallets/Wallets.Application/Wallets.Application.csproj", "src/Wallets/Wallets.Application/"]
COPY ["src/Wallets/Wallets.Domain/Wallets.Domain.csproj", "src/Wallets/Wallets.Domain/"]
COPY ["src/Wallets/Wallets.Infrastructure/Wallets.Infrastructure.csproj", "src/Wallets/Wallets.Infrastructure/"]
RUN dotnet restore "src/Wallets/Wallets.Api/Wallets.Api.csproj"
COPY . .
WORKDIR "/src/src/Wallets/Wallets.Api"
RUN dotnet build "./Wallets.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wallets.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallets.Api.dll"]
