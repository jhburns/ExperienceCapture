// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add("login", (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add("drag", { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add("dismiss", { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite("visit", (originalFn, url, options) => { ... })

// count: the number of closed sessions to create
Cypress.Commands.add('createClosedSessions', (count) => {
  [...Array(count)].forEach(() => {
    cy.request({ method: 'POST', url: '/api/v1/sessions', failOnStatusCode: true })
      .then((response) => {
        const sessionId = response.body.id;

        cy.request({ method: 'DELETE', url: `/api/v1/sessions/${sessionId}`, failOnStatusCode: true });
      });
  });
});