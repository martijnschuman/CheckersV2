import { KeyboardEventHandler } from 'react';

// Function to toggle the style of an element based on if it's valid or not
export function toggleElementStyle(element: string, action: string) {
	// Check if the input is valid
	if (action === "valid") {
		document.getElementById(element)?.classList.add("border-gray-300");
		document.getElementById(element)?.classList.remove("border-red-600");
		document.getElementById(element)?.classList.add("text-paragraph");
		document.getElementById(element)?.classList.remove("text-red-600");
		document.getElementById(element+"_label")?.classList.add("text-paragraph");
		document.getElementById(element+"_label")?.classList.remove("text-red-600");
	} else {
		document.getElementById(element)?.classList.remove("border-gray-300");
		document.getElementById(element)?.classList.add("border-red-600");
		document.getElementById(element)?.classList.remove("text-paragraph");
		document.getElementById(element)?.classList.add("text-red-600");
		document.getElementById(element+"_label")?.classList.remove("text-paragraph");
		document.getElementById(element+"_label")?.classList.add("text-red-600");
	}
}

// Function to validate the input of a string
function stringInputValid(input: string, maxLength: number) {
	const sqlInjectionPattern = /('|--|;|\/\*|\*\/|exec|execute|select|insert|update|delete|drop|create|alter)/i;
	const htmlPattern = /(<([^>]+)>)/i;
  
	return !(input.length > maxLength || sqlInjectionPattern.test(input) || htmlPattern.test(input));
}

// Function to validate the input of a password
function passwordInputValid(input: string){
	const passwordPattern = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{12,128}$/;
	return passwordPattern.test(input);
}

// Function to validate the input of an email
function emailValid(email: string, maxLength: number) {
	const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
	return emailPattern.test(email) && stringInputValid(email, maxLength);
}

// Function to validate the input of a number
function numberInputValid(input: string, maxLength: number) {
    const numberPattern = /^[0-9]+$/;
    return numberPattern.test(input) && stringInputValid(input, maxLength);
}

// Function to handle the change of a string input
export const handleStringChange = (maxLength: number): KeyboardEventHandler<HTMLInputElement> => (event) => {
    // Get the input element
    const element = event.target as HTMLInputElement;
    // Check if the input is valid
    const isValid = stringInputValid(element.value, maxLength);

    // Toggle element style based on validity
    toggleElementStyle(element.id, isValid ? "valid" : "invalid");
};

// Function to handle the change of an email input
export const handleEmailChange: KeyboardEventHandler<HTMLInputElement> = (event) => {
	// Get the input element
	const element = event.target as HTMLInputElement;
	// Check if the input is valid
	const isValid = emailValid(element.value, 128);

	// Toggle element style based on validity
	toggleElementStyle(element.id, isValid ? "valid" : "invalid");
}

// Function to handle the change of a password input
export const handlePasswordChange: KeyboardEventHandler<HTMLInputElement> = (event) => {
	// Get the input element
	const element = event.target as HTMLInputElement;
	// Check if the input is valid
	const isValid = passwordInputValid(element.value);

	// Toggle element style based on validity
	toggleElementStyle(element.id, isValid ? "valid" : "invalid");
}

export const handleNumberChange = (maxLength: number): KeyboardEventHandler<HTMLInputElement> => (event) => {
    // Get the input element
    const element = event.target as HTMLInputElement;
    // Check if the input is valid
    const isValid = numberInputValid(element.value, maxLength);

    // Toggle element style based on validity
    toggleElementStyle(element.id, isValid ? "valid" : "invalid");
};
