// TODO: add a way to check date sort, only length is checked now

describe('Sort', () => {
  it('Can sort by oldest first.', () => {
    cy.createClosedSessions(10)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=sort-dropdown]')
          .click();

        cy.injectThenCheck();
      })
      .then(() => {
        cy.get('[data-cy=session-sort-Oldest-First]')
          .click();

        cy.checkA11y();
      })
      .then(() => {
        cy.get('[data-cy=session-date]');
      })
      .then((dateElements) => {
        assert.equal(dateElements.length, 10, 'Sessions do not have the correct length.');
      });
  });

  it('Is by default sorted by newest first.', () => {
    cy.createClosedSessions(10)
      .then(() => {
        cy.visit("/home/sessions");
      })
      .then(() => {
        cy.get('[data-cy=session-link]');
      })
      .then((dateElements) => {
        assert.equal(dateElements.length, 10, 'Sessions do not have the correct length.');
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

        cy.injectThenCheck();
      })
      .then(() => {
        cy.get('[data-cy=session-sort-Alphabetically]')
          .click();

        cy.checkA11y();
      })
      .then(() => {
        cy.get('[data-cy=session-link]');
      })
      .then((idElements) => {
        const ids = idElements.toArray().map((id) => id.text);

        assert.equal(ids.length, 10, 'Sessions do not have the correct length.');

        for (let i = 0; i < ids.length - 1; i++) {
          assert.isTrue(ids[i] <= ids[i + 1], 'Sessions are not sorted alphabetically.');
        }
      });
  });
});