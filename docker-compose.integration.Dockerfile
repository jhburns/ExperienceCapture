FROM docker/compose:1.25.0-alpine

WORKDIR /app

COPY . .

# Brute-force, but it works
# Shell scripts don't for some reason

RUN cp ./Server/template.server.info.env ./Server/server.info.env && \
	cp ./Deploy/template.deploy.info.env ./Deploy/deploy.info.env && \
	cp ./Deploy/template.env ./Deploy/.env
	cp ./Server/template.env ./Server/.env

RUN docker-compose \
	-f ./Server/docker-compose.yaml \
	-f ./Server/docker-compose.override.yaml \
	config

RUN docker-compose \
	-f ./Server/docker-compose.yaml \
	-f ./Server/docker-compose.production.yaml \
	config

RUN docker-compose \
	-f ./Server/docker-compose.production.infra.yaml \
	config

RUN docker-compose \
	-f ./Deploy/docker-compose.yaml \
	config
