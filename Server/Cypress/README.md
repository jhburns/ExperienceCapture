# Cypress

Cypress is an end-to-end testing framework, [more information.](https://www.cypress.io/how-it-works)
Currently only Chrome is used as a browser to test with.

## Running locally

Install node: https://nodejs.org/en/download/ .
Do the command `npm run open` from this folder and selects which tests to run.

## Data seeding

It is recommenced to delete all of your database data locally with `docker volume rm server_ec-db-volume`. Because you may have different data locally, cleaning out the database is the easiest way to make sure the context is consistent between tests.

## Running in Docker

From this folder: `docker-compose up cy`.