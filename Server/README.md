# Server

## Setup

Install [Docker](https://docs.docker.com/v17.09/engine/installation/) along with [Docker Compose](https://docs.docker.com/compose/install/).

Then run `docker-compose build`, and thats it.

## Using

To run each of the following parts:

- API: `docker-compose up api` starts the api server, and `Ctrl-C` stops it.
- Database: should be started alongside api, but `docker-compose down` also stops it.
- Exporter: `docker-compose run exporter` which starts it interactively on the command line.
- WebUI: `docker-compose up web` starts the frontend in development mode, meaning with hot reload.
- Reverse-Proxy (Caddy): `docker-compose up rp` with start the reverse proxy, and almost all of the rest of the stack.

#### Legacy 
- ExampleAnalyzer: `docker-compose run -e filename="[CHANGE THIS]" analyzer` with the file you want to analyze.

##### Data

The *data/* folder hold all output from each service. The base of the folder holds raw output, while
the *processed/* folder is where Processor outputs. Keep in mind that data after being processed
have a filename like *processed.GHTY78QW.json*. 