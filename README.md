# Create and populate app settings

Create and populate `appsettings.Development.json` based from copy of `appsettings.json`.

# Create development certificate 

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

# Build Docker image

```bash
docker build -t filmfreakapi .
```
# Run API from container

```bash
docker run -it --rm -p 5054:5054 --network=host --name filmfreakapi filmfreakapi
```

`--network=host`

For development database on host machine and when referring localhost IP in connection string use: 

# Swagger UI

When container is running, Swagger UI should response in:

https://localhost:5054/swagger/index.html



