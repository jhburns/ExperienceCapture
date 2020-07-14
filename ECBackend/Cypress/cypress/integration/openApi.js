describe('OpenAPI', () => {
  it('Can be fetched.', () => {
    cy.request('/api/v1/openapi/');
  });

  it('Has a UI.', () => {
    cy.visit('/api/v1/openapi/ui')
      .then(() => {
        cy.get('[data-cy=leave-link]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=go-home]');
      });
  });
});