FROM hadolint/hadolint:v1.17.3-debian
SHELL ["/bin/bash", "-o", "pipefail", "-c"]
WORKDIR /app

COPY . . 

# Sort used to make output discrete and eaiser to debug
RUN find . -name "*Dockerfile*" -print0 \
  | sort -z \
  | xargs -0 --max-lines=1 hadolint