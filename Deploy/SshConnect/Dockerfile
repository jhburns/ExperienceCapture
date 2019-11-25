FROM ubuntu:xenial-20191024

WORKDIR /keys
RUN apt-get update; apt-get -y install openssh-client

CMD ["sh", "-c", "ssh -o StrictHostKeyChecking=no -i id_rsa -l ec-debug $aws_host_location"]