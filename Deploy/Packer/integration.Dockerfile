FROM hashicorp/packer:1.4.5 as builder

WORKDIR /deploy
RUN apk update \
    && apk add --no-cache \
    ansible~=2.7 \
    rsync~=3.1.3 \
    openssh-client~=7 \
    && rm -rf /tmp/* \
    && rm -rf /var/cache/apk/* \
    && pip3 install \
    ansible-lint==4.1.0

COPY .ansible-lint build.json playbook.yaml tests.py ./

RUN packer validate build.json
RUN ansible-lint playbook.yaml