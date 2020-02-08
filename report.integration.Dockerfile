FROM golang:1.13.7-buster

RUN go get -u github.com/boyter/scc

WORKDIR /app
COPY . .

RUN scc --include-ext js,cs,yaml,dockerfile,md,gitignore,sh,css,html --sort complexity --no-cocomo
RUN scc --include-ext js,cs,yaml,dockerfile,md,gitignore,sh,css,html --sort complexity --by-file --no-cocomo