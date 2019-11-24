FROM ubuntu:xenial-20191024

RUN apt-get update; apt-get -y install openssh-client

CMD ssh-keygen -b 2048 -q -t rsa -N '' -f /keys/id_rsa