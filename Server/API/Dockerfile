#- Name: Jonathan Hirokazu Burns
#- ID: 2288851
#- email: jburns@chapman.edu
#- Course: 353-01
#- Assignment: Submission #2
#- Purpose: Builds Receiver environment

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
ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.6.0/wait /wait
RUN chmod +x /wait

WORKDIR /app
EXPOSE 80

COPY --from=build-env /app/out .
COPY appsettings.json .
CMD ["dotnet", "API.dll"]