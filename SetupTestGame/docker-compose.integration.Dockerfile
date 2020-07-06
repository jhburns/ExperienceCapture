FROM docker/compose:1.25.0-alpine

WORKDIR /app

COPY . .

# Brute-force, but it works
# Shell scripts don't for some reason

RUN cp ./Deploy/template.deploy.env ./Deploy/.deploy.env && \
	cp ./Deploy/template.env ./Deploy/.env && \
	cp ./ECBackend/template.env ./ECBackend/.env

RUN docker-compose \
	-f ./ECBackend/docker-compose.yaml \
	-f ./ECBackend/docker-compose.override.yaml \
	config

RUN docker-compose \
	-f ./ECBackend/docker-compose.yaml \
	-f ./ECBackend/docker-compose.swarm.yaml \
	-f ./ECBackend/docker-compose.swarm.production.yaml \
	config

RUN docker-compose \
	-f ./ECBackend/docker-compose.yaml \
	-f ./ECBackend/docker-compose.swarm.yaml \
	-f ./ECBackend/docker-compose.swarm.staging.yaml \
	config

RUN docker-compose \
	-f ./ECBackend/docker-compose.infra.early.yaml \
	config

RUN docker-compose \
	-f ./ECBackend/docker-compose.infra.early.yaml \
	-f ./ECBackend/docker-compose.infra.yaml \
	config

RUN docker-compose \
	-f ./Deploy/docker-compose.yaml \
	config
