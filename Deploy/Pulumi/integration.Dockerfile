# Download Pulumi and extract from zip
FROM alpine:3.9.4 as pulumi-download

WORKDIR /download
RUN wget -O pulumi.tar.gz https://get.pulumi.com/releases/sdk/pulumi-v2.1.0-linux-x64.tar.gz
RUN tar -zxf pulumi.tar.gz

# Copy over project files
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-bionic
WORKDIR /app

COPY --from=pulumi-download /download/pulumi /usr/local/bin
RUN pulumi plugin install resource aws v2.2.0 --exact --logtostderr

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . .

# Pre-building so it doesn't happen everytime at runtime
RUN dotnet build -nologo . -p:GHA_BUILD=True 

# entrypoint may have windows line ending so it has to be sanitized
RUN chmod +x entrypoint.sh \
    && sed -i -e 's/\r$//' entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]