describe('My First Test', () => {
  beforeEach(() => {
    // Sign In
    cy.visit("/");
    cy.get('[data-cy=go-home]').click();
  })

  it('Does not do much!', () => {
    expect(true).to.equal(true);
  })
})