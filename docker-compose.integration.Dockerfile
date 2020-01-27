FROM docker/compose:1.25.0-alpine

WORKDIR /app

COPY . .

# Brute-force, but it works
# Shell scripts don't for some reason

RUN cp ./Deploy/template.deploy.env ./Deploy/.deploy.env && \
	cp ./Deploy/template.env ./Deploy/.env && \
	cp ./Server/template.env ./Server/.env

RUN docker-compose \
	-f ./Server/docker-compose.yaml \
	-f ./Server/docker-compose.override.yaml \
	config

RUN docker-compose \
	-f ./Server/docker-compose.yaml \
	-f ./Server/docker-compose.swarm.yaml \
	-f ./Server/docker-compose.swarm.production.yaml \
	config

RUN docker-compose \
	-f ./Server/docker-compose.yaml \
	-f ./Server/docker-compose.swarm.yaml \
	-f ./Server/docker-compose.swarm.staging.yaml \
	config

RUN docker-compose \
	-f ./Server/infrastructure/docker-compose.early.yaml \
	config

RUN docker-compose \
	-f ./Server/infrastructure/docker-compose.early.yaml \
	-f ./Server/infrastructure/docker-compose.regular.yaml \
	config

RUN docker-compose \
	-f ./Deploy/docker-compose.yaml \
	config
