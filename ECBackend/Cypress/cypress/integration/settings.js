describe('Settings Page', () => {
  it('Signs out.', () => {
    cy.visit("/home/settings");

    cy.get('[data-cy=sign-out]')
      .click();
  });

  it('Deletes the account.', () => {
    cy.visit("/home/settings");

    cy.get('[data-cy=delete-account]')
      .click();
  });

  it('Deletes other user\'s account.', () => {
    cy.visit("/home/settings");

    cy.get('[data-cy=delete-others]')
      .click();
  });

  it('Gets a new sign-up link.', () => {
    cy.visit("/home/settings");

    cy.get('[data-cy=new-sign-up]')
      .click()
      .then(() => {
        cy.get('[data-cy=sign-up-link]')
          .then((link) => {
            const url = link.text();

            // A 409 conflict is expected, as there can only
            // Be the shimmed user
            cy.visit(url);
          });
      });
  });
});