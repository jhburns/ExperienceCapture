FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-bionic AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./source/API.csproj /app/source/
COPY ./test/test.csproj /app/test/
RUN dotnet restore ./source/API.csproj && dotnet restore ./test/test.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out -nologo -p:GHA_BUILD=True
RUN dotnet test

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.3-bionic

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.6.0/wait /wait
RUN chmod +x /wait

WORKDIR /app
COPY --from=build-env /app/out .
CMD ["dotnet", "API.dll"]