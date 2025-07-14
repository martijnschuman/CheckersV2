
describe('Tests various pars of the register page', () => {
    it('The page has a title, inputs and a button', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');
        cy.get("#show-signin").click();

        cy.contains('Registreren');
        cy.get('input[name="username"]').should('be.visible');
        cy.get('input[name="email"]').should('be.visible');
        cy.get('input[name="password"]').should('be.visible');
        cy.get('input[name="password_repeat"]').should('be.visible');
    })

    it('The page has working input validation', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');
        cy.get("#show-signin").click();
        cy.contains('Registreren');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="email"]').type('test');
        cy.get('input[name="password"]').type('test');
        cy.get('input[name="password_repeat"]').type('test');

        cy.get('#form-submit').click();

        cy.contains('Er is een fout opgetreden bij het verzenden van het formulier. Controleer het formulier!');
    })

    it('The page valdiates of password and password_repeat are the same', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');
        cy.get("#show-signin").click();
        cy.contains('Registreren');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="email"]').type('test@test.nl');
        cy.get('input[name="password"]').type('Ontwikkeling123@');
        cy.get('input[name="password_repeat"]').type('Ontwikkeling123');

        cy.get('#form-submit').click();

        cy.contains('ERROR! De wachtwoorden komen niet overeen.');
    })

    it('Users can register and show success', () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');
        cy.get("#show-signin").click();
        cy.contains('Registreren');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="email"]').type('test@test.nl');
        cy.get('input[name="password"]').type('Ontwikkeling123@');
        cy.get('input[name="password_repeat"]').type('Ontwikkeling123@');

        cy.get('#form-submit').click();

        cy.contains('Account registratie gelukt!');
    })
})