FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-alpine3.10 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out -nologo


# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-alpine3.10

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.6.0/wait /wait
RUN chmod +x /wait

WORKDIR /app
EXPOSE 80

COPY --from=build-env /app/out .
CMD ["dotnet", "API.dll"]