describe('Navbar', () => {
  it('Toggles.', () => {
    cy.visit("/home/start");

    cy.get('[data-cy=menu-hamburger]')
      .click()
      .then(() => {
        cy.get('[data-cy=menu-collapse]')
          .should('have.class', 'show');
      })
      .then(() => {
        cy.get('[data-cy=menu-hamburger]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-collapse]')
          .should('not.have.class', 'show');
      });
  });

  it('Navigates between links collapsed.', () => {
    cy.visit('/')
      .then(() => {
        cy.get('[data-cy=go-home]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-hamburger]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-link-collapse]');
      })
      .then((links) => {
        assert.strictEqual(links.length, 3, "There aren't the correct number of menu links.");

        // Yes, this is messy, but its the only practical way to test each menu link
        cy.wrap(links[1])
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-hamburger]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-link-collapse]');
      })
      .then((links2) => {
        cy.wrap(links2[2])
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-hamburger]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-link-collapse]');
      })
      .then((links3) => {
        cy.wrap(links3[1])
          .click();
      });
  });

  it('Navigates between links expanded.', () => {
    cy.viewport(1024, 768)
      .then(() => {
        cy.visit('/');
      })
      .then(() => {
        cy.get('[data-cy=go-home]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-link-expand]');
      })
      .then((links) => {
        assert.strictEqual(links.length, 3, "There aren't the correct number of menu links.");

        // Yes, this is messy, but its the only practical way to test each menu link
        cy.wrap(links[1])
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-link-expand]');
      })
      .then((links2) => {
        cy.wrap(links2[2])
          .click();
      })
      .then(() => {
        cy.get('[data-cy=menu-link-expand]');
      })
      .then((links3) => {
        cy.wrap(links3[1])
          .click();
      });
  });

  it('Has a link on the brand.', () => {
    cy.visit("/home/start");

    cy.get('[data-cy=menu-brand]')
      .click();
  });
});