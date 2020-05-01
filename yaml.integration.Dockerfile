FROM ubuntu:bionic-20200403

RUN apt-get update \
 && apt-get install --no-install-recommends -y \
	python3-pip=9.0.* \
	python3-setuptools=39.0.* \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

RUN pip3 install --quiet yamllint==1.19.0

WORKDIR /app

COPY . . 

RUN yamllint --strict .