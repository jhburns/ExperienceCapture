FROM ubuntu:xenial-20191024

RUN apt-get update \
 && apt-get install --no-install-recommends -y \
	python3-pip=8.1.* \
	python3-setuptools=20.7.* \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

RUN pip3 install --quiet yamllint==1.19.0

WORKDIR /app

Copy . . 

RUN yamllint --strict .