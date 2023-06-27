FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY *.sln .
COPY Domain/*.csproj ./Domain/
COPY Auth/*.csproj ./Auth/
COPY Application/*.csproj ./Application/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY RestApi/*.csproj ./RestApi/
COPY Tests/*.csproj ./Tests/

RUN dotnet restore

COPY . .

# rename appsettings.Production.json to appsettings.json
RUN mv RestApi/appsettings.Production.json RestApi/appsettings.json

RUN dotnet build

FROM build AS publish
WORKDIR /app/RestApi

RUN dotnet publish -c Release -o out

# copy development certificate and key
#COPY *.pem /app/

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /app
COPY --from=publish /app/RestApi/out ./
EXPOSE 8080 
ENTRYPOINT ["dotnet", "RestApi.dll", "--environment=Production", "--urls=http://locahost:8080"]
