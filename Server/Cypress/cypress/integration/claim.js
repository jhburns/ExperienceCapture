describe('navbar tests.', () => {
  it('can get a claim and redeem it.', () => {
    cy.request({ method: 'POST', url: '/api/v1/authorization/claims', failOnStatusCode: true })
      .then((response) => {
        cy.visit(`/signInFor?claimToken=${response.body.claimToken}`);
      });
  });
});