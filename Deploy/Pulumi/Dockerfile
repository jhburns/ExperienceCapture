
# Download Pulumi and extract from zip
FROM alpine:3.9.4 as pulumi-download

WORKDIR /download
RUN wget -O  pulumi.tar.gz https://get.pulumi.com/releases/sdk/pulumi-v1.6.0-linux-x64.tar.gz
RUN tar -zxvf pulumi.tar.gz

# Copy over project files
FROM mcr.microsoft.com/dotnet/core/sdk:3.0.101-alpine3.9 
WORKDIR /app

RUN apk add --no-cache libc6-compat=1.1.20-r5 && rm -rf /tmp/*
COPY --from=pulumi-download /download/pulumi /usr/local/bin

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./

#ENTRYPOINT ["pulumi", "login", "&&"]

#CMD ["pulumi", "up"]