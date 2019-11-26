FROM mcr.microsoft.com/dotnet/core/sdk:2.2.402-alpine3.9 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
#COPY *.csproj ./
#RUN dotnet restore

# Copy everything else and build
#COPY . ./
#RUN dotnet publish -c Release -o out

# Build runtime image
FROM FROM ubuntu:xenial-20191024 as pulumi-download

RUN wget -O  pulumi.tar.gz https://get.pulumi.com/releases/sdk/pulumi-v1.6.0-linux-x64.tar.gz
RUN tar -zxvf pulumi.tar.gz

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.7-alpine3.9

WORKDIR /app

#COPY --from=build-env /app/out .
CMD ["sh"]