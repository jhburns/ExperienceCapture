describe('Completed sessions page', () => {
  it('Can archive then unarchive a session.', () => {
    cy.createClosedSessions(1)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-row]');
      })
      .then((row) => {
        assert.isNotNull(row, 'Completed sessions should display a session row.');
        assert.isTrue(row.length === 1, 'Completed sessions should display only one session.');

        cy.get('[data-cy=session-button]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-row]').should('not.exist');
      })
      .then(() => {
        cy.get('[data-cy=archive-link]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-row]');
      })
      .then((row) => {
        assert.isNotNull(row, 'Archived sessions should display a session row.');
        assert.isTrue(row.length === 1, 'Archived sessions should display only one session.');

        cy.get('[data-cy=session-button]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-row]').should('not.exist');
      })
      .then(() => {
        cy.get('[data-cy=sessions-link]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-row]');
      })
      .then((row) => {
        assert.isNotNull(row, 'Completed sessions should display a session row.');
        assert.isTrue(row.length === 1, 'Completed sessions should display only one session.');
      });
  });
});