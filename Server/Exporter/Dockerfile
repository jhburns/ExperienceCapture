FROM mcr.microsoft.com/dotnet/core/sdk:2.2.402-alpine3.9 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.7-alpine3.9
WORKDIR /app

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.6.0/wait /wait
RUN chmod +x /wait

COPY --from=build-env /app/out .
CMD ["dotnet", "Exporter.dll"]