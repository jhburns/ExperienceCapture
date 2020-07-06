describe('Settings Page', () => {
  it('Allows the user to sign-out..', () => {
    cy.visit("/home/settings");

    cy.get('[data-cy=sign-out]')
      .click();
  });
});