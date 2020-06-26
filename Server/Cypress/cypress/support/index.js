// ***********************************************************
// This example support/index.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands';

// Alternatively you can use CommonJS syntax:
// require('./commands')

// Disable screenshots for all tests
// This can only be configured in code, not cypres.json
Cypress.Screenshot.defaults({
  screenshotOnRunFailure: false
});

beforeEach(() => {
  // Seed
  // Wipe the database first
  cy.exec('docker-compose -f docker-compose.clone.yaml --project-name server exec -T db mongo ec --eval "db.dropDatabase();"');

  // Visit the admin signup path
  cy.visit("/admin?password=validationIsTurnOff");

  // Wait until it can sign In
  cy.get('[data-cy=go-home]');
});

