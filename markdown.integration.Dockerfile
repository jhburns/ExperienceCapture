FROM pipelinecomponents/markdownlint:0.6.1

COPY . .

RUN mdl --style custom_style.rb --warnings .