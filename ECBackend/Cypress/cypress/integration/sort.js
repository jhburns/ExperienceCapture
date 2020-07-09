import moment from 'moment';

describe('Session Sort', () => {
  it('Is sorted by newest first.', () => {
    cy.createClosedSessions(10)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-date]')
      })
      .then((dateElements) => {
        const dates = dateElements.toArray().map((date) => moment(date.text));

        assert.equal(dates.length, 10, 'Sessions do not have the correct length.');

        for (let i = 0; i < dates.length - 1; i++) {
          assert.isTrue(dates[i] <= dates[i + 1], 'Sessions are not in newest first order.');
        }
      });
  });

  it('Can sort alphabetically.', () => {
    cy.createClosedSessions(10)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=sort-dropdown]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-sort-Alphabetically]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-link]')
      })
      .then((idElements) => {
        const ids = idElements.toArray().map((id) => id.text);

        assert.equal(ids.length, 10, 'Sessions do not have the correct length.');

        for (let i = 0; i < ids.length - 1; i++) {
          assert.isTrue(ids[i] <= ids[i + 1], 'Sessions are not able to be sorted alphabetically.');
        }
      });
  });

  it('Can sort by oldest first.', () => {
    cy.createClosedSessions(10)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=sort-dropdown]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-sort-Oldest-First]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=session-date]')
      })
      .then((dateElements) => {
        const dates = dateElements.toArray().map((date) => moment(date.text));

        assert.equal(dates.length, 10, 'Sessions do not have the correct length.');

        for (let i = 0; i < dates.length - 1; i++) {
          assert.isTrue(dates[i] >= dates[i + 1], 'Sessions are not in oldest first order.');
        }
      });
  });
});