describe('Create claim flow', () => {
  it('creates a new claim for an existing policy', () => {
    cy.visit('/');
    cy.get('input[placeholder="Username"]').clear().type('admin');
    cy.get('input[placeholder="Password"]').clear().type('Password123!');
    cy.contains('Sign in').click();
    cy.contains('Claims', { timeout: 10000 }).click();
    // assume there is a button to create a new claim
    cy.contains('New Claim').click();
    cy.get('input[name="policyNumber"]').type('POL-1001');
    cy.get('textarea[name="description"]').type('Test claim created by e2e');
    cy.contains('Submit').click();
    cy.contains('Claim submitted', { timeout: 10000 });
  });
});
