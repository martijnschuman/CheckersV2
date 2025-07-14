"use client"
import Cookies from 'js-cookie'
import QRCode from "react-qr-code";
import Nav from '@/components/Nav';
import { useState, useEffect } from 'react'
import Button from '@/components/misc/Button';
import Spinner from '@/components/misc/Spinner';
import FormInput from '@/components/misc/FormInput';
import { useAlert } from '@/providers/AlertProvider';
import { toggleElementStyle, handleNumberChange } from '@/services/formValidationService';

export default function Page() {
    const [isLoading, setIsLoading] = useState(true);

    const [setupMFA, showSetupMFA] = useState<boolean>(true);
    const [disableMFA, showDisableMFA] = useState<boolean>(false);

    const [MultiFactorKey, setMultiFactorKey] = useState<string>("");
    const [MultiFactorQRCode, setMultiFactorQRCode] = useState<string>("");

    const { setAlertDangerVisible, setAlertSuccessVisible } = useAlert();

    const GetMultiFactorKey = async () => {
        try {
            const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Authenticate/getMFA', {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${Cookies.get('accessToken')}`
                },
            });

            // Handle response if necessary
            const data = await response.json();

            // If the response is 200 OK
            if (response.status === 200) {
                if(data.status === "Success" && data.multiFactorKey !== null){
                    setMultiFactorKey(data.multiFactorKey);
                    setMultiFactorQRCode(data.multiFactoryKeyQR);
                    showSetupMFA(true);
                    setIsLoading(false);
                }else if(data.status === "ERROR" && data.multiFactorKey === null){
                    showSetupMFA(false);
                    showDisableMFA(true);
                    setIsLoading(false);
                }
                
                if(process.env.NEXT_PUBLIC_DEBUG){
                    console.log("2FA setup: ");
                    console.log(data);
                }
            }
        } catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
				console.error("GetMultiFactorKey: ");
				console.error(error);
                setAlertDangerVisible([true, "Something went wrong while trying to get the MultiFactorKey."]);
			}
		} 
    };

    useEffect(() => {
        GetMultiFactorKey();
    }, []);

    // Function to verify the form inputs
    function verifyFormInputs(formData : FormData){
		// Access the form data using the get method of the FormData object
		const multiFactorKey = formData.get("multiFactorKey");

		// Log the response if debug mode is enabled
		if(process.env.NEXT_PUBLIC_DEBUG){
			console.log("verifyFormInputs: ");
			console.log(multiFactorKey);
		}

		// Check if the form data is valid
		if (!multiFactorKey) {
			setAlertDangerVisible([true, "ERROR! multiFactorKey ongeldig."]);

			// Toggle element style for invalid form data
			toggleElementStyle("multiFactorKey", multiFactorKey ? "valid" : "invalid");

			return false;
		}

		return true;
	}

    // Function to handle the form submission to enable MFA
    const handleEnableMFA = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        try{
            const formData = new FormData(event.currentTarget);

            if(verifyFormInputs(formData)){
                const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Authenticate/enableMFA', {
                    method: 'POST',
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${Cookies.get('accessToken')}`
                    },
                    body: JSON.stringify({
                        MultiFactorKey: formData.get("multiFactorKey")
                    }),
                });
    
                // Handle response if necessary
                const data = await response.json();
    
                if(process.env.NEXT_PUBLIC_DEBUG){
                    console.log("handleEnableMFA: ");
                    console.log(data);
                }
    
                // If the response is 200 OK
                if (response.status === 200) {
                    setAlertSuccessVisible([true, "Succes! 2FA is geactiveerd."]);
                    showSetupMFA(false);
                    showDisableMFA(true);
                } else {
                    setAlertDangerVisible([true, "Er is iets misgegaan bij het activeren van 2FA."]);
                }
            }
        } catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
				setAlertDangerVisible([true, "ERROR! Er is iets misgegaan bij het activeren van 2FA."]);
				console.error("handleJoinGame: ");
				console.error(error);
			}
		} finally {
			setTimeout(() => {
				setAlertSuccessVisible([false, ""]);
				setAlertDangerVisible([false, ""]);
			}, 6000);
		}
    }

    // Function to handle the form submission to disable MFA
    const handleDisableMFA = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        try{
            const formData = new FormData(event.currentTarget);

            if(verifyFormInputs(formData)){
                const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Authenticate/disableMFA', {
                    method: 'POST',
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${Cookies.get('accessToken')}`
                    },
                    body: JSON.stringify({
                        MultiFactorKey: formData.get("multiFactorKey")
                    }),
                });
    
                // Handle response if necessary
                const data = await response.json();
    
                if(process.env.NEXT_PUBLIC_DEBUG){
                    console.log("handleJoinGame: ");
                    console.log(data);
                }
    
                // If the response is 200 OK
                if (response.status === 200) {
                    setAlertSuccessVisible([true, "Succes! 2FA is gedeactiveerd."]);
    
                    showSetupMFA(true);
                    showDisableMFA(false);
                    GetMultiFactorKey();
                } else {
                    setAlertDangerVisible([true, "Er is iets misgegaan bij het deactivateren van 2FA."]);
                }
            }
        } catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
				setAlertDangerVisible([true, "ERROR! Er is een fout opgetreden bij het joinen van het damspel. Controleer de uitnodigingscode!"]);
				console.error("handleJoinGame: ");
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



	return (
        <div className="h-screen flex items-center justify-center">
            

            <Nav />

            <div className='w-96 -mt-2 content-element shadow-lg rounded-xl p-3 md:px-8 md:py-4'>
				<h1 className="text-4xl text-center mt-2 mb-8 font-semibold">Multi-factor authentication setup</h1>

                { isLoading && 
                    <Spinner className='mt-6 mb-0' />
                }
                { setupMFA && !isLoading && 
                    <div>
                        <p>Om 2FA te activeren moet je de QR code scannen of de volgende code invoeren in een 2FA app.</p>

                        <p className='mt-1 -mb-3'>Code: {MultiFactorKey}</p>
                        <div style={{ background: 'white', padding: '16px' }}>
                            {<QRCode value={MultiFactorQRCode} />}
                        </div>

                        <form onSubmit={handleEnableMFA}> 
                            <p>Voer in het onderstaande veld de gegenereerde 2FA code in, waarna 2FA wordt geactiveerd.</p>
                            <FormInput type="number" id="multiFactorKey" label="Code" name="multiFactorKey" customClass='mt-1' onKeyUpCapture={handleNumberChange(6)}/>
                            <Button type="submit" id="enableMFA" value='MFA activeren'/>
                        </form>
                    </div>
                }

                { disableMFA &&
                    <div>
                        <form onSubmit={handleDisableMFA}> 
                            <p>Voer in het onderstaande veld de gegenereerde 2FA code in, waarna 2FA wordt gedeactiveerd.</p>
                            <FormInput type="number" id="multiFactorKey" label="Code" name="multiFactorKey" customClass='mt-1' onKeyUpCapture={handleNumberChange(6)}/>
                            <Button type="submit" id="disableMFA" value='MFA deactiveren'/>
                        </form>
                    </div>
                }
			</div>
        </div>
    )
}