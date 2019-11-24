FROM ubuntu:xenial-20191024

WORKDIR /keys
RUN apt-get update; apt-get -y install openssh-client

CMD ["ssh-keygen", "-b", "2048", "-q", "-t", "rsa", "-N", "", "-f", "./id_rsa"]