"use client"
import Cookies from 'js-cookie';
import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';

import Nav from "@/components/Nav";
import Button from '@/components/misc/Button';
import Spinner from '@/components/misc/Spinner';
import FormInput from '@/components/misc/FormInput';
import { useAlert } from '@/providers/AlertProvider';
import Statistics from "@/components/dashboard/Statistics";
import CheckersGame from "@/components/dashboard/CheckersGame";
import createHubConnection from '@/services/signalRService';
import { getUserIdFromToken } from '@/services/tokenService';
import { faTrophy, faEquals, faSkull } from '@fortawesome/free-solid-svg-icons';
import { toggleElementStyle, handleNumberChange } from '@/services/formValidationService';


export default function Page() {
    const router = useRouter()
    
    // States
    const [isLoading, setIsLoading] = useState(true);
	const { setAlertDangerVisible, setAlertSuccessVisible } = useAlert();

    const [games, setGames] = useState([]);
    const [wins, setWins] = useState([]);
    const [draws, setDraws] = useState([]);
    const [losses, setLosses] = useState([]);

    const [gameCode, setGameCode] = useState();

    // Gets dashboard data
    useEffect(() => {
        const response = fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/dashboard/getAllGames', {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${Cookies.get('accessToken')}`
            },
        });

        response.then((res) => {
            res.json().then((data) => {
                setGames(data["checkerGames"])
                setWins(data["checkerGameStats"]["wins"])
                setDraws(data["checkerGameStats"]["draws"])
                setLosses(data["checkerGameStats"]["losses"])
                setIsLoading(false);

                if(process.env.NEXT_PUBLIC_DEBUG){
                    console.log("getGames: ");
                    console.log(data);
                }
            })
            .catch((error) => {
                // Handle timeout error
                if(process.env.NEXT_PUBLIC_DEBUG){
                    console.log("getGames: ");
                    console.log(error);
                }

                setAlertDangerVisible([true, "Er is iets misgegaan bij het ophalen van de spellen."]);
                // Set isLoading to false and handle the error state
                setIsLoading(false);
            });
        })
    }, [])

    const initializeSignalR = (gameCode: number) => {
        const connection = createHubConnection(process.env.NEXT_PUBLIC_API_BASE_URL+'/checkersHub', Cookies.get('accessToken') || '');

        connection
            .start()
            .then(() => {
                console.log('SignalR connection established.');

                connection.on('GameStarted', (gameState) => {
                    console.log('Game state updated:', gameState);

                    // Redirect to the game page
                    router.push('/play');
                });

                // Join the group associated with the game
                connection.invoke('JoinGameGroup', gameCode)
                    .then(() => {
                        console.log('Joined game group ' + gameCode + ' successfully.');
                    })
                    .catch((error) => {
                        console.error('Error joining game group:', error);
                    });
            })
            .catch((error) => {
                console.error('Error connecting to SignalR:', error);
            });

        return () => {
            connection.stop();
        };
    };

    // Creates a game and shows the game code
    const handleCreateGame = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        try {
            const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Checkers/createGame', {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${Cookies.get('accessToken')}`
                },
            });

            // Handle response if necessary
            const data = await response.json();

            if(process.env.NEXT_PUBLIC_DEBUG){
                console.log("handleCreateGame: ");
                console.log(data);
            }

            // If the response is 200 OK
            if (response.status === 200) {
                setGameCode(data["gameCode"]);
                setAlertSuccessVisible([true, "Succes! Damspel aangemaakt met code: " + data["gameCode"]]);

                // Connect to SingalR group with the gameCode as name
                initializeSignalR(data["gameCode"]);
            } else {
                setAlertDangerVisible([true, "Er is iets misgegaan bij het aanmaken van het spel."]);
            }
        } catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
				setAlertDangerVisible([true, "ERROR! Er is een fout opgetreden bij het maken van het damspel!"]);
				console.error("handleCreateGame: ");
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

    // Function to verify the form inputs
    function verifyFormInputs(formData : FormData){
		// Access the form data using the get method of the FormData object
		const gameCode = formData.get("gameCode");

		// Log the response if debug mode is enabled
		if(process.env.NEXT_PUBLIC_DEBUG){
			console.log("verifyFormInputs: ");
			console.log(gameCode);
		}

		// Check if the form data is valid
		if (!gameCode) {
			setAlertDangerVisible([true, "ERROR! GameCode ongeldig."]);

			// Toggle element style for invalid form data
			toggleElementStyle("gameCode", gameCode ? "valid" : "invalid");

			setIsLoading(false);
			return false;
		}

		return true;
	}

    // Joins a game
    const handleJoinGame = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        try{
            const formData = new FormData(event.currentTarget);

            if(verifyFormInputs(formData)){
                const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Checkers/joinGame', {
                    method: 'POST',
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${Cookies.get('accessToken')}`
                    },
                    body: JSON.stringify({
                        GameCode: formData.get("gameCode")
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
                    setAlertSuccessVisible([true, "Succes! Aangesloten bij damspel: " + data["gameCode"]]);
    
                    // Connect to SingalR group with the gameCode as name
                    initializeSignalR(data["gameCode"]);
                    // Redirect to the game page
                    router.push('/play');
                } else {
                    setAlertDangerVisible([true, "Er is iets misgegaan bij het aansluiten bij het damspel met code: " + data["gameCode"]]);
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
        <section>
            <Nav />
            
            <div className="flex justify-center mt-2 md:mt-2">
                <div className="grid grid-cols-2 gap-4">
                    <form className="col-span-2 sm:col-span-1" id="createForm" method='POST' onSubmit={handleCreateGame}>
                        <Button id="createGame" type="submit" value="Spel aanmaken" customClass='mb-0 sm:mb-3'/>
                    </form>

                    <form className="col-span-2 sm:col-span-1 flex items-center" id="joinForm" method='POST' onSubmit={handleJoinGame}>
                        <FormInput id="gameCode" type="number" name="gameCode" label="Spelcode" customClass="mb-2 sm:mb-4" onKeyUpCapture={handleNumberChange(8)}/>
                        <Button id="joinGame" type="submit" value="Spel aansluiten" customClass="-mt-2"/>
                    </form>

                </div>
            </div>

            { gameCode &&
                <div className="flex justify-center">
                    <div className="p-4 mb-4 text-sm text-green-800 rounded-lg bg-green-50 dark:bg-gray-800 dark:text-green-400" role="alert">
                        <span className="font-medium">Succes!</span> Damspel aangemaakt met code: {gameCode}. Deel deze code met je tegenstander om het spel te starten. Je hebt 30 minuten om het spel te starten.
                    </div>
                </div>
            }

            { isLoading && 
                <div className="flex justify-center align-middle h-screen">
                    <Spinner />
                </div>
            }

            { !isLoading && 
                <div>
                    <div className="flex justify-center mb-4 md:mb-6 md:mt-2">
                        <div className="grid grid-cols-3 w-full md:grid-cols-3 gap-4 lg:gap-8 ml-2 mr-2 sm:w-8/12">
                            <Statistics titel="Gewonnen" value={wins.toString()} icon={faTrophy} iconHeight={30} iconColor="text-green-400"/>
                            <Statistics titel="Gelijk" value={draws.toString()} icon={faEquals} iconHeight={35} iconColor="text-yellow-400 -mt-1"/>
                            <Statistics titel="Verloren" value={losses.toString()} icon={faSkull} iconHeight={30} iconColor="text-red-600"/>
                        </div>
                    </div>

                    <div className="flex justify-center">
                        <div className="grid grid-cols-1 w-full gap-4 ml-2 mr-2 sm:w-8/12">                            
                            {games.map((game, index) => (
                                <CheckersGame
                                    key={index}
                                    Id={index.toString()}
                                    Color="bg-blue-400" 
                                    Status={game["game"]["gameState"] == 0 ? "Open" : game["game"]["gameState"] == 1 ? "Bezig" : game["game"]["gameState"] == 2 && game["game"]["winnerId"] == getUserIdFromToken() ? "Gewonnen" : game["game"]["gameState"] == 3 && game["game"]["winnerId"] == getUserIdFromToken() ? "Gewonnen" : game["game"]["gameState"] == 5 ? "Geannuleerd" : "Verloren"} 
                                    GameCode={game["game"]["gameCode"]}
                                    StonesWhite={game["playerOneRemainingPieces"]} 
                                    StonesBlack={game["playerTwoRemainingPieces"]} 
                                    PlayerOne={game["playerOneUserName"]} 
                                    PlayerOneId={game["game"]["playerOneId"]}
                                    PlayerTwo={game["playerTwoUserName"]} 
                                    PlayerTwoId={game["game"]["playerTwoId"]}
                                    StartDateTime={game["game"]["startDateTime"]} 
                                    EndDateTime={game["game"]["endDateTime"]}
                                />
                            ))}
                        </div>
                    </div>
                </div>
            }
        </section>
    )
}