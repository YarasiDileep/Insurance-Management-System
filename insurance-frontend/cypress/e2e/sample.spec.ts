describe('Insurance App - smoke', () => {
  it('loads the login page', () => {
    cy.visit('/');
    cy.contains('Login');
  });
});
