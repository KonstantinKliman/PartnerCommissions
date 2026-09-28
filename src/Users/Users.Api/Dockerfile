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
COPY ["src/Users/Users.Api/Users.Api.csproj", "src/Users/Users.Api/"]
COPY ["src/Users/Users.Application/Users.Application.csproj", "src/Users/Users.Application/"]
COPY ["src/Users/Users.Domain/Users.Domain.csproj", "src/Users/Users.Domain/"]
COPY ["src/Users/Users.Infrastructure/Users.Infrastructure.csproj", "src/Users/Users.Infrastructure/"]
RUN dotnet restore "src/Users/Users.Api/Users.Api.csproj"
COPY src/Shared/ src/Shared/
COPY src/Users/ src/Users/
RUN dotnet publish "src/Users/Users.Api/Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Users.Api.dll"]
