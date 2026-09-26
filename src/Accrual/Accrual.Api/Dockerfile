FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Accrual/Accrual.Api/Accrual.Api.csproj", "src/Accrual/Accrual.Api/"]
COPY ["src/Accrual/Accrual.Application/Accrual.Application.csproj", "src/Accrual/Accrual.Application/"]
COPY ["src/Accrual/Accrual.Domain/Accrual.Domain.csproj", "src/Accrual/Accrual.Domain/"]
COPY ["src/Accrual/Accrual.Infrastructure/Accrual.Infrastructure.csproj", "src/Accrual/Accrual.Infrastructure/"]
RUN dotnet restore "src/Accrual/Accrual.Api/Accrual.Api.csproj"
COPY . .
WORKDIR "/src/src/Accrual/Accrual.Api"
RUN dotnet build "./Accrual.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Accrual.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accrual.Api.dll"]
