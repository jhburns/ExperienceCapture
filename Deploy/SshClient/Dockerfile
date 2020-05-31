FROM ubuntu:bionic-20200403

WORKDIR /keys
RUN apt-get update \
	&& apt-get install -y --no-install-recommends \
	openssh-client=1:7.* \
	&& apt-get clean \
	&& rm -rf /var/lib/apt/lists/*