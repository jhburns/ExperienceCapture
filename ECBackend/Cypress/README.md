[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Cypress%20Tests/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Cypress+Tests%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Cypress%20Lint/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Cypress+Lint%22)

# Cypress

Cypress is an end-to-end testing framework, [more information.](https://www.cypress.io/how-it-works)
Currently only Chrome is used as a browser to test with.

## Running locally

Install node: https://nodejs.org/en/download/ . Version 81 of Chrome is also recommend to be installed, in order to make tests consistent. Download specific versions of chrome here: https://chromium.cypress.io/ . To run tests, first start the server with the command `docker-compose up rp`.
Then, run the command `npm run open` from this folder and selects which tests to run.

## Data seeding

In order to make sure that testing is consistent, all data is deleted between runs. Backup local MongoDB data if it important, before running cypress.

## Running in Docker

From this folder: `docker-compose up cy`. All test information will be printed to the console.