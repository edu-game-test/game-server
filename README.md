# game-server

ASP.NET Core 8 game backend. Clean Architecture: Domain / Application / Infrastructure / API.

## Local development

1. `docker compose -f docker-compose.emulator.yml up -d` — Firestore emulator on :8681
2. Set the emulator host env var:
   - bash: `export FIRESTORE_EMULATOR_HOST=localhost:8681`
   - PowerShell: `$env:FIRESTORE_EMULATOR_HOST="localhost:8681"`
3. `dotnet run --project src/GameServer.API` → https://localhost:5001/swagger
4. `dotnet test` — integration tests require steps 1 and 2

## Running tests

```bash
# Unit tests only (no emulator needed)
dotnet test tests/GameServer.Unit.Tests

# Integration tests (start emulator first)
docker compose -f docker-compose.emulator.yml up -d
export FIRESTORE_EMULATOR_HOST=localhost:8681
dotnet test tests/GameServer.Integration.Tests
```
