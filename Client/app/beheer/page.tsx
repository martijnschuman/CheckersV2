"use client"
import Cookies from 'js-cookie';
import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';

import Nav from "@/components/Nav";
import Spinner from '@/components/misc/Spinner';
import { useAlert } from '@/providers/AlertProvider';
import CheckersGame from "@/components/dashboard/CheckersGame";
import { getUserRoleFromToken } from '@/services/tokenService';

export default function Page() {
    const router = useRouter()

    // States
    const [isLoading, setIsLoading] = useState(true);
	const { setAlertDangerVisible, setAlertSuccessVisible } = useAlert();
    const [cancelableGames, setCancelableGames] = useState([]);
    const [allGames, setAllGames] = useState([]);

    // Gets dashboard data
    useEffect(() => {
        if(getUserRoleFromToken() != "Beheerder"){
            router.push("/dashboard");
        }

        GetGames();
    }, [])

    // Get all games
    const GetGames = () => {
        const response = fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/admin/getAllGames', {
            method: 'GET',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${Cookies.get('accessToken')}`
            },
        });

        response.then((res) => {
            res.json().then((data) => {
                setCancelableGames(data["cancelableGames"])
                setAllGames(data["remainingGames"])
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
    }

    // Cancel a game by its gameId
    const handleCancelGame = async (gameId : number) => {
        try{
            const response = await fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Admin/cancelGame', {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${Cookies.get('accessToken')}`
                },
                body: JSON.stringify({
                    GameId: gameId
                }),
            });

            // Handle response if necessary
            const data = await response.json();

            if(process.env.NEXT_PUBLIC_DEBUG){
                console.log("handleCancelGame: ");
                console.log(data);
            }

            // If the response is 200 OK
            if (response.status === 200) {
                setAlertSuccessVisible([true, "Succes! Damspel geannuleerd met code: " + gameId]);
                GetGames();
            } else {
                setAlertDangerVisible([true, "Er is iets misgegaan bij het annuleren van het damspel met code: " + gameId]);
            }
        } catch (error) {
			// Handle error if necessary
			// Log the response if debug mode is enabled
			if(process.env.NEXT_PUBLIC_DEBUG){
                setAlertDangerVisible([true, "Er is iets misgegaan bij het annuleren van het damspel met code: " + gameId]);
				console.error("handleCancelGame: ");
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

            <h1 className='text-center text-3xl mt-2 underline'>Damspellen beheren</h1>

            { isLoading && 
                <div className="flex justify-center align-middle h-screen">
                    <Spinner />
                </div>
            }

            { !isLoading && 
                <div>
                    <div className="flex justify-center">
                        <div className="grid grid-cols-1 w-full gap-4 ml-2 mr-2 sm:w-8/12">
                            {cancelableGames.length > 0 &&
                                <h2 className='text-center text-xl -mb-4 mt-2'>Games die geannuleerd kunnen worden</h2>
                            }

                            {cancelableGames.map((game, index) => (
                                    <CheckersGame
                                        key={index}
                                        Id={index.toString()}
                                        Color="bg-blue-400" 
                                        Status={game["game"]["gameState"] == 0 ? "Open" : game["game"]["gameState"] == 1 ? "Bezig" : game["game"]["gameState"] == 2 ? "Speler 1 gewonnen" : game["game"]["gameState"] == 3 ? "Speler 2 gewonnen" : game["game"]["gameState"] == 5 ? "Geannuleerd" : "Gelijk"} 
                                        GameCode={game["game"]["gameCode"]}
                                        StonesWhite={game["playerOneRemainingPieces"]} 
                                        StonesBlack={game["playerTwoRemainingPieces"]} 
                                        PlayerOne={game["playerOneUserName"]} 
                                        PlayerOneId={game["game"]["playerOneId"]}
                                        PlayerTwo={game["playerTwoUserName"]} 
                                        PlayerTwoId={game["game"]["playerTwoId"]}
                                        StartDateTime={game["game"]["startDateTime"]} 
                                        EndDateTime={game["game"]["endDateTime"]}
                                        cancelAction={() => handleCancelGame(game["game"]["gameId"])}
                                    />
                                ))
                            }
                            
                            {allGames.length > 0 &&
                                <h2 className='text-center text-xl -mb-4 mt-2'>Alle Damspellen</h2>
                            }

                            {allGames.length > 0 &&
                                allGames.map((game, index) => (
                                    <CheckersGame
                                        key={index}
                                        Id={index.toString()}
                                        Color="bg-blue-400" 
                                        Status={game["game"]["gameState"] == 0 ? "Open" : game["game"]["gameState"] == 1 ? "Bezig" : game["game"]["gameState"] == 2 ? "Speler 1 gewonnen" : game["game"]["gameState"] == 3 ? "Speler 2 gewonnen" : game["game"]["gameState"] == 5 ? "Geannuleerd" : "Gelijk"} 
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
                                ))
                            }
                        </div>
                    </div>
                </div>
            }
        </section>
    )
}