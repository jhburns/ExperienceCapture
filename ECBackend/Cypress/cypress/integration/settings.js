describe('Settings Page', () => {
  it('Signs out.', () => {
    cy.toSettings()
      .then(() => {
        cy.injectThenCheck();

        cy.get('[data-cy=sign-out]')
          .click();

        cy.injectThenCheck();
      });
  });

  it('Deletes the account.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=delete-account]')
          .click();

        cy.injectThenCheck();
      })
      .then(() => {
        cy.get('[data-cy=cancel-delete]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=delete-account]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=confirm-delete]')
          .click();

        cy.checkA11y();
      });
  });

  it('Deletes other user\'s account.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=delete-others]')
          .click();

        cy.injectThenCheck();
      });
  });

  it('Gets a new sign-up link.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=new-sign-up]')
          .click();

        cy.injectThenCheck();
      })
      .then(() => {
        cy.get('[data-cy=sign-up-link]');
      })
      .then((link) => {
        const url = link.text();

        // A 409 conflict is expected, as there can only
        // Be one shimmed user
        cy.visit(url);
      })
      .then(() => {
        cy.get('[data-cy=already-notify]');

        cy.injectThenCheck();
      });
  });
});