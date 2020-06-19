FROM hashicorp/packer:1.4.5 as builder

WORKDIR /deploy
RUN apk update \
    && apk add --no-cache \
    ansible~=2.7 \
    rsync~=3.1.3 \
    openssh-client~=7 \
    && rm -rf /tmp/* \
    && rm -rf /var/cache/apk/*

COPY . .

CMD ["build", "-on-error=ask", "build.json"]