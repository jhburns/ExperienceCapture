# Download Pulumi and extract from zip
FROM alpine:3.9.4 as pulumi-download

WORKDIR /download
RUN wget -O  pulumi.tar.gz https://get.pulumi.com/releases/sdk/pulumi-v1.7.0-linux-x64.tar.gz
RUN tar -zxf pulumi.tar.gz

# Copy over project files
FROM mcr.microsoft.com/dotnet/core/sdk:3.0.101-bionic
WORKDIR /app

COPY --from=pulumi-download /download/pulumi /usr/local/bin
RUN pulumi plugin install resource aws v1.10.0 --exact

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./

RUN dotnet build -nologo . -p:GHA_BUILD=True 