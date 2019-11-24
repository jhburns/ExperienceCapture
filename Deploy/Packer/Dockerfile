FROM hashicorp/packer:1.4.5 as builder

RUN apk add ansible=2.7.14-r0 && rm -rf /tmp/* && rm -rf /var/cache/apk/*

COPY build.json playbook.yaml ./

CMD ["build", "-on-error=ask", "build.json"]