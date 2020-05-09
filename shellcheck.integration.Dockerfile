FROM koalaman/shellcheck-alpine:v0.7.0

SHELL ["/bin/ash", "-eo", "pipefail", "-c"]
WORKDIR /app

COPY . .

ENV SHELLCHECK_OPTS="--exclude=SC1017"
RUN find . -name "*.sh" -print0 \
    | sort -z \
    | xargs -0 -n 1 shellcheck