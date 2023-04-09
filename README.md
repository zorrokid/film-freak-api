# FilmFreakApi

## Requirements

- .net 7.0 SDK
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

## Run in development environment

## Build Docker image

```bash
docker build -t filmfreakapi .
```

## Run API from container

```bash
docker run -it --rm -p 5054:5054 --network=host --name filmfreakapi filmfreakapi
```

`--network=host`

For development database on host machine and when referring localhost IP in connection string use: 

## Swagger UI

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