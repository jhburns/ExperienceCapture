describe('404 page', () => {
  it('Navigates home.', () => {
    cy.visit('/this/should/never/exist/')
      .then(() => {
        cy.injectThenCheck();

        cy.get('[data-cy=sign-in-link]')
          .click();
      });
  });
});