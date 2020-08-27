describe('Claim Page', () => {
  it('Can get a claim and redeem it.', () => {
    cy.request({ method: 'POST', url: '/api/v1/authentication/claims', failOnStatusCode: true })
      .then((response) => {
        const encodedClaim = encodeURIComponent(response.body.claimToken);
        cy.visit(`/?claimToken=${encodedClaim}`);
      });

    /* TODO: re-enable this or some type of navigation check
      .then(() => {
        cy.get('[data-cy=go-root]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=go-home]');
      });
    */
  });
});