FROM hashicorp/packer:1.4.5 as builder

# Prevents pip from complaining about being out of date
# And installs reviewdog to a folder that is in the PATH
ENV PIP_DISABLE_PIP_VERSION_CHECK=1 BINDIR=/usr/local/bin
SHELL ["/bin/ash", "-eo", "pipefail", "-c"]

WORKDIR /deploy

RUN wget -O - -q https://raw.githubusercontent.com/reviewdog/reviewdog/master/install.sh| sh -s v0.9.17

RUN apk update \
    && apk add --no-cache \
    ansible=2.7.16-r0 \
    rsync=3.1.3-r1 \
    openssh-client=7.9_p1-r6 \
    && rm -rf /tmp/* \
    && rm -rf /var/cache/apk/* \
    && pip3 install ansible-lint==4.1.0

COPY .ansible-lint build.json playbook.yaml ./

RUN echo $CI_REPO_OWNER

RUN packer validate build.json
RUN ansible-lint playbook.yaml | reviewdog -f=ansible-lint -reporter=github-pr-check