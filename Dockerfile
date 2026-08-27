FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY GameServer.sln .
COPY src/GameServer.API/GameServer.API.csproj src/GameServer.API/
COPY src/GameServer.Application/GameServer.Application.csproj src/GameServer.Application/
COPY src/GameServer.Domain/GameServer.Domain.csproj src/GameServer.Domain/
COPY src/GameServer.Infrastructure/GameServer.Infrastructure.csproj src/GameServer.Infrastructure/

RUN dotnet restore

COPY src/ src/
RUN dotnet publish src/GameServer.API/GameServer.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GameServer.API.dll"]
