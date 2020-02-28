
# Download Pulumi and extract from zip
FROM alpine:3.9.4 as pulumi-download

WORKDIR /download
RUN wget -O  pulumi.tar.gz https://get.pulumi.com/releases/sdk/pulumi-v1.11.1-linux-x64.tar.gz
RUN tar -zxf pulumi.tar.gz

# Copy over project files
FROM mcr.microsoft.com/dotnet/core/sdk:3.0.101-bionic
WORKDIR /app

# TODO: make this step more efficient, by only copying over needed files
COPY --from=pulumi-download /download/pulumi /usr/local/bin
RUN pulumi plugin install resource aws v1.10.0 --exact

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . .

# Pre-building so it doesn't happen everytime at runtime
RUN dotnet build -nologo .

# entrypoint may have windows line ending so it has to be sanitized 
RUN chmod +x entrypoint.sh \
 && sed -i -e 's/\r$//' entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]