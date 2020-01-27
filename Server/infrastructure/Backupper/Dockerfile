FROM mongo:4.2.2-bionic

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    unzip=6.0-21ubuntu1 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

ADD "https://d1vvhvl2y92vvt.cloudfront.net/awscli-exe-linux-x86_64.zip" awscliv2.zip
RUN unzip awscliv2.zip && ./aws/install