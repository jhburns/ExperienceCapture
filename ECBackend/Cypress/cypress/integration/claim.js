/* Removing test for now, because this page is getting reworked anyway,
 * And this test will become outdated
describe('Claim Page', () => {
  it('Can get a claim and redeem it.', () => {
    cy.request({ method: 'POST', url: '/api/v1/authentication/claims', failOnStatusCode: true })
      .then((response) => {
        const encodedClaim = encodeURIComponent(response.body.claimToken);
        cy.visit(`/?claimToken=${encodedClaim}`);
      })
      .then(() => {
        cy.get('[data-cy=go-root]')
          .click();
      })
      .then(() => {
        cy.get('[data-cy=go-home]');
      });
  });
});
*/