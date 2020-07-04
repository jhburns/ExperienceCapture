describe('Claim Page', () => {
  it('Can get a claim and redeem it.', () => {
    cy.request({ method: 'POST', url: '/api/v1/authentication/claims', failOnStatusCode: true })
      .then((response) => {
        cy.visit(`/signInFor?claimToken=${response.body.claimToken}`)
          .then(() => {
            cy.get('[data-cy=go-root]')
              .click();
          });
      });
  });
});