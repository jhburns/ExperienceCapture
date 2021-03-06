FROM golang:1.13.7-buster

RUN go get -u github.com/boyter/scc

WORKDIR /app
COPY . .

RUN scc \
    --include-ext js,cs,yaml,dockerfile,md,gitignore,sh,css,html,py,txt,r \
    --sort complexity \
    --no-cocomo \
    --no-min-gen \
    --wide

RUN scc \
    --include-ext js,cs,yaml,dockerfile,md,gitignore,sh,css,html,py,txt,r \
    --sort complexity \
    --by-file \
    --no-min-gen \
    --wide