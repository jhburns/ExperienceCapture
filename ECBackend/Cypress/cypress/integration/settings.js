describe('Settings Page', () => {
  it('Signs out.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=sign-out]')
          .click();
      });
  });

  it('Deletes the account.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=delete-account]')
          .click();
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
      });
  });

  it('Deletes other user\'s account.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=delete-others]')
          .click();
      });
  });

  it('Gets a new sign-up link.', () => {
    cy.toSettings()
      .then(() => {
        cy.get('[data-cy=new-sign-up]')
          .click();
      })
      .then(() => {
        // Clicking this will hang cypress
        cy.get('[data-cy=new-link-copy]');
      })
      .then(() => {
        cy.get('[data-cy=new-link]');
      })
      .then((link) => {
        const url = link.text();

        // A 409 conflict is expected, as there can only
        // Be one shimmed user
        cy.visit(url);
      })
      .then(() => {
        cy.get('[data-cy=already-notify]');
      });
  });
});