![https://github.com/jhburns/ExperienceCapture/workflows/Receiver%20Integration/badge.svg]

# Server

## Setup

Install [Docker](https://docs.docker.com/v17.09/engine/installation/) along with [Docker Compose](https://docs.docker.com/compose/install/).

Then run `docker-compose build`, and thats it.

## Using

To run each of the following parts:

- Receiver: `docker-compose up web` starts the server, and `Ctrl-C` stops it.
- Database: should be started alongside Reveiver, but `docker-compose down` also stops it.


#### Legacy 
- Processor: `docker-compose run -e filename="[CHANGE THIS]" processor` with the file you want to process.
- ExampleAnalyzer: `docker-compose run -e filename="[CHANGE THIS]" analyzer` with the file you want to analyze.

##### Data

The *data/* folder hold all output from each service. The base of the folder holds raw output, while
the *processed/* folder is where Processor outputs. Keep in mind that data after being processed
have a filename like *processed.GHTY78QW.json*. 