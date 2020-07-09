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

  it('Navigates between links.', () => {
    cy.visit("/home/start");

    cy.get('[data-cy=menu-hamburger]')
      .click()
      .then(() => {
        cy.get('[data-cy=menu-link]')  
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
        cy.get('[data-cy=menu-link]');
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
        cy.get('[data-cy=menu-link]');
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