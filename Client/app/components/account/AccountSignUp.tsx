"use client"
import React, { useState } from 'react';
import Button from '@/components/misc/Button';
import Spinner from '@/components/misc/Spinner';
import FormInput from '@/components/misc/FormInput';
import { useAlert } from '@/providers/AlertProvider';
import { toggleElementStyle, handleStringChange, handleEmailChange, handlePasswordChange } from '@/services/formValidationService';


interface accountSignInProps {
    setShowSignIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const AccountSignUp: React.FC<accountSignInProps> = (props) => {
    // States
	const [isLoading, setIsLoading] = useState<boolean>(false);
	const { setAlertDangerVisible, setAlertSuccessVisible } = useAlert();

	// Function to verify the form inputs
    function verifyFormInputs(formData : FormData){
		// Access the form data using the get method of the FormData object
		const username = formData.get("username");
		const email = formData.get("email");
		const password = formData.get("password");
		const password_repeat = formData.get("password_repeat");

		// Log the response if debug mode is enabled
		if(process.env.NEXT_PUBLIC_DEBUG){
			console.log("verifyFormInputs: ");
			console.log(username, password);
		}

		// Check if the form data is valid
		if (!username || !email || !password || !password_repeat) {
			setAlertDangerVisible([true, "ERROR! Vul alle velden in."]);

			// Toggle element style for invalid form data
			toggleElementStyle("username", username ? "valid" : "invalid");
			toggleElementStyle("email", email ? "valid" : "invalid");
			toggleElementStyle("password", password ? "valid" : "invalid");
			toggleElementStyle("password_repeat", password_repeat ? "valid" : "invalid");

			setIsLoading(false);
			return false;
		}

		if(password !== password_repeat){
			setAlertDangerVisible([true, "ERROR! De wachtwoorden komen niet overeen."]);

			// Toggle element style for invalid form data
			toggleElementStyle("password", "invalid");
			toggleElementStyle("password_repeat", "invalid");

			setIsLoading(false);
			return false;
		}

		return true;
	}

    // Function to handle the form submission
	const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		// Set loading to true when the request starts
		setIsLoading(true); 
		
		// Tries to send the form data
		try {
			// Get the form data
			const formData = new FormData(event.currentTarget);

			// Check if the form data is valid
			if(verifyFormInputs(formData)){
				// Send the form data
				const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Authenticate/signUp', {
					method: 'POST',
					headers: {
						"Content-Type": "application/json"
					},
					body: JSON.stringify({
						username: formData.get("username"),
						email: formData.get("email"),
						password: formData.get("password"),
					}),
				});

				// Check if the response is successful
				if (response.ok && response.status === 200) {
					// Log the response if debug mode is enabled
					if(process.env.NEXT_PUBLIC_DEBUG){
						console.log("login submit: ");
						console.log(response);
					}

					// Reset the form
					var form = document.getElementById("signup") as HTMLFormElement;
					form.reset();

					// Show success alert
					setAlertSuccessVisible([true, "Account registratie gelukt!"]);

					// Show the sign in form
					props.setShowSignIn(true);
				} else{
					const data = await response.json();
					if(process.env.NEXT_PUBLIC_DEBUG){
						console.log("login submit: ");
						console.log(data);
					}

					setAlertDangerVisible([true, "Er is een fout opgetreden bij het verzenden van het formulier. Controleer het formulier!"]);
				}
			}else{
				setAlertDangerVisible([true, "Er is een fout opgetreden bij het verzenden van het formulier. Controleer het formulier!"]);
			}
		} catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
				setAlertDangerVisible([true, "Er is een fout opgetreden bij het verzenden van het formulier. Controleer het formulier!"]);
				console.error("onSubmit: ");
				console.error(error);
			}
		} finally {
			// Resets states
			setIsLoading(false);

			setTimeout(() => {
				setAlertSuccessVisible([false, ""]);
				setAlertDangerVisible([false, ""]);
			}, 6000);
		}
	}

	// Function to handle the show sign in button
	const handleShowSignIn = () => {
		// False is show sign in
		props.setShowSignIn(true);	
	}

	return (
		<section>
			<div className='w-96 -mt-2 content-element shadow-lg rounded-xl p-3 md:px-8 md:py-4'>
				<h1 className="text-4xl text-center mt-2 mb-8 font-semibold">Registreren</h1>

				{isLoading && <Spinner/>}
				
				<form className="mx-auto" id="signup" onSubmit={handleSubmit} method="POST">
					<FormInput type="text" id="username" name="username" label="Gebruikersnaam" onKeyUpCapture={handleStringChange(60)} />
					<FormInput type="text" id="email" name="email" label="Email" onKeyUpCapture={handleEmailChange} />
					<FormInput type="password" id="password" name="password" label="Wachtwoord" onKeyUpCapture={handlePasswordChange} />
					<FormInput type="password" id="password_repeat" name="password_repeat" label="Wachtwoord herhalen" onKeyUpCapture={handlePasswordChange} />

					<div className='flex flex-col items-center'>
						<div className="flex justify-between space-x-2">
							<Button type='submit' id="form-submit" value='Registreren'/>
							<Button type='submit' id="show-signin" value='Terug naar inloggen' submitAction={handleShowSignIn}/>
						</div>
					</div>
				</form>
			</div>
		</section>
	);
};

export default AccountSignUp