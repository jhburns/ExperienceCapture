# TODO: change to Ubuntu version 18.04
FROM ubuntu:xenial-20191024

WORKDIR /keys
RUN apt-get update \
	&& apt-get install -y --no-install-recommends \
	openssh-client=1:7.2p2-4ubuntu2.8 \
	&& apt-get clean \
	&& rm -rf /var/lib/apt/lists/*