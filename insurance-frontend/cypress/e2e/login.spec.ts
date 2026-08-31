describe('Login flow', () => {
  it('allows a seeded admin to sign in', () => {
    cy.visit('/');
    cy.get('input[placeholder="Username"]').clear().type('admin');
    cy.get('input[placeholder="Password"]').clear().type('Password123!');
    cy.contains('Sign in').click();
    // after successful login the app reloads and dashboard should be visible
    cy.contains('Dashboard', { timeout: 10000 });
  });
});
