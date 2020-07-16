describe('Session page', () => {
  it('Can export and download a session.', () => {
    cy.createClosedSessions(1)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-link]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-export]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-download]');
      })
      .then((button) => {
        assert.isNotNull(button, 'After exporting, there is no download button.');
      });
  });

  it('Toggles the help prompt.', () => {
    cy.createClosedSessions(1)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-link]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=about-prompt]')
          .trigger('mouseover');
      })
      .then(() => {
        cy.get('[data-cy=about-tooltip]').should('be.visible');
      })
      .then(() => {
        cy.get('[data-cy=about-prompt]')
          .trigger('mouseout');
      })
      .then(() => {
        cy.get('[data-cy=about-tooltip]').should('be.not.visible');
      });
  });
});