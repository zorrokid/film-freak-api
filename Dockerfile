FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY FilmFreakApi.csproj .
RUN dotnet restore --use-current-runtime  

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false --no-restore

# copy development certificate and key
COPY *.pem /app/

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "FilmFreakApi.dll", "--environment=Development"]
