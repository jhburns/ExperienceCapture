describe('Root Page', () => {
  it('Goes to the correct home.', () => {
    cy.visit("/");

    cy.injectThenCheck();

    cy.get('[data-cy=go-home]')
      .click()
      .then(() => {
        cy.url()
          .should((u) => {
            assert.include(u, "/home/start", "The home button goes to the wrong url.");
          });
      });
  });

  it('Allows the user to sign-out.', () => {
    cy.visit("/");

    cy.get('[data-cy=sign-out]')
      .click()
      .then(() => {
        cy.get('[data-cy=sign-out]').should('not.exist');
        cy.get('[data-cy=go-home]').should('not.exist');
      })
      .then(() => {
        // This does nothing, but is still good to check
        cy.get('[data-cy=sign-in]')
          .click();

        cy.injectThenCheck();
      });
  });
});