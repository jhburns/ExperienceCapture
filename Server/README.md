# Server

## Setup

### Env Files

Follow the tutorial [here](https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Partial-Deploy.md#create-and-copy-environment-files) and [here](https://github.com/jhburns/ExperienceCapture/blob/master/Documentation/Partial-Deploy.md#change-build-arg).

### Build

Run `docker-compose build` to develop locally.

### Running

- API: `docker-compose up rp api` starts the api server.
- Database: should be started alongside any service that needs it, but `docker-compose down` also stops it.
- Exporter: `docker-compose run -e exporter_session_id=XXXX exporter` starts the exporter, with it exporting the given exporter_session_id (required). It will exit when done. Keep in mind the API also can start an exporter job.
- WebUI: `docker-compose up rp web` starts the front-end in development mode, meaning with hot reload.
- Reverse-Proxy (Caddy): `docker-compose up rp` with start the reverse proxy, and almost all of the rest of the stack. This may be broken, so use the API's command if it is.
- Backupper: `docker-compose up bu` which will dump MongoDB's data into S3.

`Ctrl-C` stops any service, that was named in the `docker-compose up`. To stop all services, run `docker-compose down`.

## About Docker Compose 

This project heavily uses Docker-compose to manage the application stack. Here is what each compose file is for:

- `docker-compose.yaml`: the base file, used for development, staging, and production.
- `docker-compose.override.yaml`: only for development, see https://docs.docker.com/compose/extends/ .
- `docker-compose.swarm.yaml`: for production and staging, hence the 'swarm' name.
- `docker-compose.swarm.production.yaml`: only for production.
- `docker-compose.swarm.staging.yaml`: only for staging.
- `docker-compose.infra.yaml`: infrastructure from production/staging meaning different from application services.
- `docker-compose.infra.early.yaml` infrastructure that has to be applied before other application/infrastructure code.

For specific information about how all of this works together, see `Deploy/Packer/playbook.yaml`.

## Overview

To learn more about each service visit its respective folder this is only a description of how it all fits together.

#### Routing

The server uses a reverse proxy (Caddy) to route traffic between the static front-end and the API back-end.
A request with `/api/v1/` in its path will be routed to the API server, while everything else
goes to the front-end. Some examples:

```
/api/v1/sessions/ -> API
/api/ -> WebUI
/home/sessions/ -> WebUI
```

The WebUI is a Single-Page Application so routing to a specific webpage is done server side.

If the a request is routed to the API server, it then uses internal logic to routes the request to a specific endpoint.
The API server also has three services it depends on, the Exporter service, [MongoDB](https://www.mongodb.com/), and
[Minio](https://min.io/). MongoDB is used for storing game session data and general purpose application data.
The Exporter also requires MongoDB and Minio so it can convert database documents and store them as a zip file.
Minio is an object store, which is only used to store/serve exported game sessions zips.

During development, routing is done without load balancing as there is only one instance of each service used.
However, during production/staging the API, Caddy, and WebUI are replicated so [Docker Swarm](https://docs.docker.com/engine/swarm/) has to load balance between each replica. This is done with a non-configurable round-robin load balancing algorithm.

#### Infrastructure Services

There are a couple of services not involved with serving application traffic, all
of them only run in production/staging:

- Backupper, which is scheduled to dump MongoDB into S3 everyday at 2 AM. Warning: Backups are not automatically deleted.
- Docker Registry, which is required to use custom built images in Docker Swarm.
- Swarm Cronjob, which runs jobs given their crontab. It schedules the Backupper and Garbage Collector services.
- Garbage Collector, which deletes stopped containers, dangling images, etc. This prevents the Virtual Machine from running out of space.

#### Exporter

The exporter is a unique service as it isn't continuously running, or started by the cron service.
Instead, when its api endpoint is called (`GET /api/v1/sessions/{id}/export/`) a docker container will be
started and then exit when done exporting that `id`'s session. The container will then be deleted by the Garbage
Collector.