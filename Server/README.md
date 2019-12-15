# Server

## Setup

Install [Docker](https://docs.docker.com/v17.09/engine/installation/) along with [Docker Compose](https://docs.docker.com/compose/install/).

Then run `docker-compose build`, and that's it.

## Using

To run each of the following parts:

- API: `docker-compose up api` starts the api server, and `Ctrl-C` stops it.
- Database: should be started alongside api, but `docker-compose down` also stops it.
- Exporter: `docker-compose run exporter` which starts it interactively on the command line.
- WebUI: `docker-compose up web` starts the front-end in development mode, meaning with hot reload.
- Reverse-Proxy (Caddy): `docker-compose up rp` with start the reverse proxy, and almost all of the rest of the stack.


##### Data

The *data/exported* folder hold all output from the Exporter. Each file has the name *{session id}.sorted.json*
and is a sort JSON array.