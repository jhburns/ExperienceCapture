# Server

## Setup

Install [Docker](https://docs.docker.com/v17.09/engine/installation/) along with [Docker Compose](https://docs.docker.com/compose/install/).

Then run `docker-compose build`, and thats it.

## Using

To run each of the following parts:
	- Receiver: `docker-compose up` starts the server, and `Ctrl-C` stops it.
	- Processor: `docker-compose run -e filename="[CHANGE THIS]" processor` with the file you want to process.
	- ExampleAnalyzer: `docker-compose run -e filename="[CHANGE THIS]" analyzer` with the file you want to analyze.