FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Wallets/Wallets.Api/Wallets.Api.csproj", "src/Wallets/Wallets.Api/"]
COPY ["src/Wallets/Wallets.Application/Wallets.Application.csproj", "src/Wallets/Wallets.Application/"]
COPY ["src/Wallets/Wallets.Domain/Wallets.Domain.csproj", "src/Wallets/Wallets.Domain/"]
COPY ["src/Wallets/Wallets.Infrastructure/Wallets.Infrastructure.csproj", "src/Wallets/Wallets.Infrastructure/"]
RUN dotnet restore "src/Wallets/Wallets.Api/Wallets.Api.csproj"
COPY src/Shared/ src/Shared/
COPY src/Wallets/ src/Wallets/
RUN dotnet publish "src/Wallets/Wallets.Api/Wallets.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Wallets.Api.dll"]
