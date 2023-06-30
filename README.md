# FilmFreakApi

## Requirements

- .net 7.x SDK
- PostgreSQL-server

## Create and populate app settings

Create and populate `appsettings.Development.json` based from copy of `appsettings.Template.json`.

```json
  "Database": {
    "ConnectionString": "Server=<db server ip>;Port=<db server port>;Database=<filmfreak db>;Username=<username>;Password=<password>",
    "AuthDbConnectionString": "Server=<db server ip>;Port=<db server port>;Database=<filmfreakauth db>;Username=<username>;Password=<password>"
  },
  "AdminCredentials": {
    "UserName": "<admin user>",
    "Email": "<admin user email>",
    "Password": "<admin password>"
  },
  "JWT": {
    "ValidAudience": "https://localhost:5054",
    "ValidIssuer": "https://localhost:5054",
    "Secret": "<token secret>",
    "ExpirationInHours": 1
  }
```

## Create development certificate 

Create self-signed develoment certificate and key using mkcert

```bash
mkcert localhost 127.0.0.1
```

This should create two files:
* `localhost+1-key.pem` 
* `localhost+1.pem`

Docker image build will copy these files to app-folder in container.
For use in Docker-container for development environment reference these files in Kestrel enpoint settings in appsettings.Development.json. 

```json
  "Kestrel": {
    "Endpoints": {
      "HttpsFromPem": {
        "Url": "https://localhost:5054",
        "Certificate": {
          "Path": "/app/localhost+1.pem",
          "KeyPath": "/app/localhost+1-key.pem"
        }
      }
    }
  }
```

## Run in development environment without Docker

1. Ensure appsettings.Development.json is created and populated with values as described above 
2. Ensure PostgreSQL is running and connection string in appsettings.Development.json is correct
3. Start the API by running `dotnet run` in the RestApi project folder
4. Open https://localhost:5054/swagger/index.html to see Swagger UI
5. You can get the needed JWT Bearer token for Swagger Authorization by using /api/Login endpoint in Swagger UI

## Run in development environment with Docker

### Build Docker image

```bash
docker build -t filmfreakapi .
```

### Run API from container

```bash
docker run -it --rm -p 5054:5054 --network=host --name filmfreakapi filmfreakapi
```

`--network=host`

For development database on host machine and when referring localhost IP in connection string use: 

### Swagger UI

When container is running, Swagger UI should response in:

https://localhost:5054/swagger/index.html

# Migrations

## AuthDb

To create migration, from solution root run:

   dotnet ef migrations add --context AuthDbContext <migration name> --project Infrastructure --startup-project RestApi --output-dir Infrastructure/Persistence/Migrations/AuthDb

To update database, from the solution root run:

  dotnet ef database update --context AuthDbContext --project RestApi

## FilmFreakDb

To create migration, from the solution root run:

  dotnet ef migrations add --context FilmFreakContext <migration name> --project Infrastructure --startup-project RestApi --output-dir Infrastructure/Persistence/Migrations/FilmFreakDb

To update database, from the solution root run:

  dotnet ef database update --context FilmFreakContext --project RestApi

To move databse to certain migration

  dotnet ef database update <migration name> --context FilmFreakContext --project RestApi

## Deployment

Uses github action workflows to deploy automatically to fly.io. 

## Data model

- A release is certain publification, a movie or collection of movies, a TV-series or a a season of TV-series as a single publification or a _release_

**TODO**: maybe release should be renamed to publification

- A collection item is always based on a release - it has always a Release relation

- Releases can have ownership 
  - they're only visible for the owner
  - only owner can modify the release

- Releases can be promoted for shared common use (IsShared flag)
  - the ownership is not cleared but the original owner cannot modify the data directly
  - promoted release is visible to other users 
  - also other users can suggest modifications

- Collection items have ownership
  - they're visible only for the owner unless owner sets a public visibility
  - only owner can modify the collection item 