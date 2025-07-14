describe('Tests the create game page', () => {
    it("The page has the create game button and an input field", () => {
        cy.visit('http://localhost:3000/account');

        cy.contains('Inloggen');

        cy.get('input[name="username"]').type('test');
        cy.get('input[name="password"]').type('Ontwikkeling123@');

        cy.get('#form-submit').click();

        cy.contains('Spel aanmaken');
        cy.contains('Spel aansluiten');

        cy.reload();

        cy.get('#createGame').click();
        cy.contains("Succes! Damspel aangemaakt met code:");
    })
})