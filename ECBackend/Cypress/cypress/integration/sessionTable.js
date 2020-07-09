import moment from 'moment';

describe('Session Table', () => {
  it('Is empty when there are no sessions.', () => {
    cy.visit("/home/sessions")
      .then(() => {
        cy.get('[data-cy=session-row]').should('not.exist')
      })
      .then(() => {
        cy.get('[data-cy=sessions-empty]')
      })
      .then((message) => {
        assert.isNotNull(message, 'Empty sessions table does not display a message.');
      });      
  });

  it('Displays a session.', () => {
    cy.request({ method: 'POST', url: '/api/v1/sessions', failOnStatusCode: true })
      .then((response1) => {
        const sessionId = response1.body.id;

        cy.request({ method: 'DELETE', url: `/api/v1/sessions/${sessionId}`, failOnStatusCode: true });
      })
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-row]');
      })
      .then((row) => {
        assert.isNotNull(row, 'Session table should display a session row.');

        cy.get('[data-cy=sessions-empty]').should('not.exist');
      });
  });

  it('Is sorted by newest first.', () => {
    cy.createClosedSessions(10)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-date]')
      })
      .then((dateElements) => {
        const dates = dateElements.map((date) => moment(date.textContent));

        for (let i = 0; i < dates.length - 1; i++) {
          assert.isTrue(dates[i] <= dates[i + 1], 'Sessions are not in newest first order.');
        }
      })
  });
});