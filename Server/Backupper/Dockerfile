FROM mongo:4.2.2-bionic

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    unzip=6.0-21ubuntu1 \
    curl=7.58.0-2ubuntu3.8 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN curl "https://d1vvhvl2y92vvt.cloudfront.net/awscli-exe-linux-x86_64.zip" -o awscliv2.zip
RUN unzip awscliv2.zip && ./aws/install