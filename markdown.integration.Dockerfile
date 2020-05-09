FROM pipelinecomponents/markdownlint:0.6.1

COPY . .

RUN mdl --style all --warnings .