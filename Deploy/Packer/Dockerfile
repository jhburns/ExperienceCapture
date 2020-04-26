FROM hashicorp/packer:1.4.5 as builder

WORKDIR /deploy
RUN apk update \
    && apk add --no-cache \
    ansible>2.7.17-r0 \
    rsync>3.1.3-r1 \
    openssh-client>7.9_p1-r6 \
    && rm -rf /tmp/* \
    && rm -rf /var/cache/apk/*

COPY build.json playbook.yaml ./

CMD ["build", "-on-error=ask", "build.json"]