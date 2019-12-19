FROM hashicorp/packer:1.4.5 as builder

# Prevents pip from complaining about being out of date
ENV PIP_DISABLE_PIP_VERSION_CHECK=1 

WORKDIR /deploy

RUN apk update && apk add --no-cache ansible=2.7.14-r0 rsync=3.1.3-r1 openssh-client=7.9_p1-r6 && rm -rf /tmp/* && rm -rf /var/cache/apk/*
RUN pip3 install ansible-lint==4.1.0

COPY .ansible-lint build.json playbook.yaml ./

RUN packer validate build.json
RUN ansible-lint playbook.yaml