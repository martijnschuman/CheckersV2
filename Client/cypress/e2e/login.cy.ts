
describe('Tests to see if the login page exists', () => {
    it('Has a title, inputs and a button', () => {
        cy.visit('http://localhost:3000/account')

        cy.contains('Inloggen')
        cy.get('input[type="text"]').should('be.visible')
        cy.get('input[type="password"]').should('be.visible')
        cy.get('input[type="number"]').should('be.visible')
        cy.get('button').should('be.visible')
    })

    it('Users can login with new user', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="password"]').type('Ontwikkeling123@');

        cy.get('#form-submit').click();

        cy.contains('Spel aanmaken');
        cy.contains('Spel aansluiten');
    })

    it('Users can go to MFA setup', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="password"]').type('Ontwikkeling123@');

        cy.get('#form-submit').click();

        cy.contains('Spel aanmaken');
        cy.contains('Spel aansluiten');

        cy.reload();

        cy.get('#dropdownDefaultButton').click();
        cy.get('#mfa').click();

        cy.contains('Multi-factor authentication setup');
    })

    it('Users can logout', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="password"]').type('Ontwikkeling123@');

        cy.get('#form-submit').click();

        cy.contains('Spel aanmaken');
        cy.contains('Spel aansluiten');

        cy.reload();

        cy.get('#dropdownDefaultButton').click();
        cy.get('#logout').click();

        cy.contains('Inloggen');
    })
})