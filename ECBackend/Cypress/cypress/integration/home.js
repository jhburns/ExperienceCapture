describe('Home page', () => {
  it('Displays ongoing sessions.', () => {
    cy.request({ method: 'POST', url: '/api/v1/sessions', failOnStatusCode: true })
      .then(() => {
        cy.visit('/home/start');
      })
      .then(() => {
        cy.get('[data-cy=session-row]');
      })
      .then((row) => {
        assert.isNotNull(row, 'Home should display a session row.');

        cy.get('[data-cy=sessions-empty]').should('not.exist');
      });
  });
});