describe('Policies page', () => {
  beforeEach(() => {
    cy.visit('/');
  });

  it('shows a list of policies after login', () => {
    // Use seeded credential helper flow from login test
    cy.get('input[placeholder="Username"]').clear().type('admin');
    cy.get('input[placeholder="Password"]').clear().type('Password123!');
    cy.contains('Sign in').click();
    cy.contains('Policies', { timeout: 10000 });
    cy.get('.policies ul li').its('length').should('be.gte', 1);
  });
});
