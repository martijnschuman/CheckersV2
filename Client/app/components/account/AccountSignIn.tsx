"use client"
import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import Cookies from 'js-cookie';

import Button from '@/components/misc/Button';
import Spinner from '@/components/misc/Spinner';
import FormInput from '@/components/misc/FormInput';
import { useAlert } from '@/providers/AlertProvider';
import { toggleElementStyle, handleStringChange } from '@/services/formValidationService';


interface AccountSignInProps {
    setShowSignIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const AccountSignIn: React.FC<AccountSignInProps> = (props) => {
	const router = useRouter()

    // States
	const [isLoading, setIsLoading] = useState<boolean>(false);
	const { setAlertDangerVisible, setAlertSuccessVisible } = useAlert();

	// Function to verify the form inputs
    function verifyFormInputs(formData : FormData){
		// Access the form data using the get method of the FormData object
		const username = formData.get("username");
		const password = formData.get("password");

		// Log the response if debug mode is enabled
		if(process.env.NEXT_PUBLIC_DEBUG){
			console.log("verifyFormInputs: ");
			console.log(username, password);
		}

		// Check if the form data is valid
		if (!username || !password) {
			setAlertDangerVisible([true, "ERROR! Vul alle velden in."]);

			// Toggle element style for invalid form data
			toggleElementStyle("username", username ? "valid" : "invalid");
			toggleElementStyle("password", password ? "valid" : "invalid");

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
				const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Authenticate/signIn', {
					method: 'POST',
					headers: {
						"Content-Type": "application/json"
					},
					body: JSON.stringify({
						username: formData.get("username"),
						password: formData.get("password"),
						MultiFactorKey: formData.get("MultiFactorKey") || "0",
					}),
				});
	
				// Handle response if necessary
				const data = await response.json();

				// Log the response if debug mode is enabled
				if(process.env.NEXT_PUBLIC_DEBUG){
					console.log("login submit: ");
					console.log(data);
				}

				// If the response is 200 OK
				if (response.status === 200) {
					// Reset the form
					var form = document.getElementById("signin") as HTMLFormElement;
					form.reset();

					// Set the access token in a cookie
					const expirationDate = new Date(data["expiration"]);
					Cookies.set('accessToken', data["token"], { expires: expirationDate, secure: true });

					// Redirect to the dashboard
					router.push('/dashboard');
				} else if (response.status === 401) {
					setAlertDangerVisible([true, "ERROR! Ongeldige combinatie van inloggegevens."]);
				} else {
					setAlertDangerVisible([true, "ERROR! Er is een fout opgetreden bij het inloggen. Probeer het later opnieuw."]);
				}
			}
		} catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
				setAlertDangerVisible([true, "ERROR! Er is een fout opgetreden bij het verzenden van het formulier. Controleer het formulier!"]);
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
		// False is show sign up
		props.setShowSignIn(false);	
	}

	return (
		<section>
			<div className='w-96 -mt-2 content-element shadow-lg rounded-xl p-3 md:px-8 md:py-4'>
				<h1 className="text-4xl text-center mt-2 mb-8 font-semibold">Inloggen</h1>

				{isLoading && <Spinner/>}
				
				<form className="mx-auto" id="signin" onSubmit={handleSubmit} method="POST">
					<FormInput type="text" id="username" name="username" label="Gebruikersnaam" onKeyUpCapture={handleStringChange(60)} />
					<FormInput type="password" id="password" name="password" label="Wachtwoord" onKeyUpCapture={handleStringChange(60)} />
					<FormInput type="number" id="MultiFactorKey" name="MultiFactorKey" label="2FA Code" onKeyUpCapture={handleStringChange(6)} />

					<div className='flex flex-col items-center'>
						<div className="flex justify-between space-x-2">
							<Button type='submit' id="form-submit" value='Inloggen'/>
							<Button type='submit' id="show-signin" value='Geen account?' submitAction={handleShowSignIn}/>
						</div>
					</div>
				</form>
			</div>
		</section>
    );
};

export default AccountSignIn;
