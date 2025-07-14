"use client"
import Cookies from "js-cookie";
import { useRouter } from 'next/navigation';
import { HubConnection } from '@microsoft/signalr';
import React, { useEffect, useState } from "react";

import Nav from "@/components/Nav";
import Button from "@/components/misc/Button";
import CheckersBoard from "@/components/play/CheckersBoard";
import createHubConnection from "@/services/signalRService";
import { getUserIdFromToken } from "@/services/tokenService";


export default function Page() {
    const router = useRouter()

    // Game states
    const [pieces, setPieces] = useState([]);
    const [gameCode, setGameCode] = useState(0);
    const [playerOne, setPlayerOne] = useState("");
    const [playerTwo, setPlayerTwo] = useState("");
    const [currentPlayer, setCurrentPlayer] = useState("");
    const [PlayerOneRemainingPieces, setPlayerOneRemainingPieces] = useState(0);
    const [PlayerTwoRemainingPieces, setPlayerTwoRemainingPieces] = useState(0);
    const [GameMessage , setGameMessage] = useState("");

    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [origin, setOrigin] = useState<[number, number] | null>(null);
    const [destination, setDestination] = useState<[number, number] | null>(null);

    useEffect(() => {
        const initializeSignalR = (gameCode : number) => {
            const newConnection = createHubConnection(process.env.NEXT_PUBLIC_API_BASE_URL+'/checkersHub', Cookies.get('accessToken') || '');
    
            newConnection
                .start()
                .then(() => {
                    console.log('SignalR connection established.');
    
                    // When the server sends a message with the name "CheckersBoardUpdated", update the game state
                    newConnection.on('CheckersBoardUpdated', (newBoard, gameState, message, currentPlayerUserName) => {
                        // Update the current player
                        setCurrentPlayer(currentPlayerUserName);
                        // Update the remaining pieces for each player
                        setPlayerOneRemainingPieces(newBoard.filter((p: any) => p.playerNumber === 1 && p.isTaken === false).length);
                        setPlayerTwoRemainingPieces(newBoard.filter((p: any) => p.playerNumber === 2 && p.isTaken === false).length);
                        // Update the game state
                        setPieces(newBoard);
                        // Update the game message
                        setGameMessage(message);
                    });

                    // If an error occurs on the server, display the error message
                    newConnection.on("CheckersBoardError", (error) => {
                        console.log("CheckersBoardError:", error);

                        // Update the game message
                        setGameMessage(error);                        
                    })
    
                    // Join the group associated with the game
                    newConnection.invoke('JoinGameGroup', gameCode)
                        .then(() => {
                            console.log('Joined game group ' + gameCode + ' successfully.');
                        })
                        .catch((error) => {
                            console.error('Error joining game group:', error);
                        });

                    setConnection(newConnection);
                })
                .catch((error) => {
                    console.error('Error connecting to SignalR:', error);
                });
    
            return () => {
                newConnection.stop();
            };
        };

        // Haal spelinfo op via API call
        const getGameInfo = () => {
            const response = fetch(process.env.NEXT_PUBLIC_API_BASE_URL+'/Checkers/getGameInfo', {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${Cookies.get('accessToken')}`
                },
            });

            response.then((res) => {
                if(res.status === 400){
                    console.log("Bad request");
                    router.push('/dashboard');
                }

                res.json().then((data) => {
                    if(data.checkerGame === null){
                        console.log("Game not found");
                        router.push('/dashboard');
                    }

                    console.log(data);
                    setGameCode(data.checkerGame.gameCode);
                    setPieces(data.checkerGame.board);
                    setCurrentPlayer(data.currentPlayerUserName);
                    setPlayerOne(data.playerOneUserName);
                    setPlayerTwo(data.playerTwoUserName);
                    setPlayerOneRemainingPieces(data.checkerGame.board.filter((p: any) => p.playerNumber === 1 && p.isTaken === false).length);
                    setPlayerTwoRemainingPieces(data.checkerGame.board.filter((p: any) => p.playerNumber === 2 && p.isTaken === false).length);

                    initializeSignalR(data.checkerGame.gameCode);
                })
                .catch((error) => {
                    // Handle timeout error
                    if(process.env.NEXT_PUBLIC_DEBUG){
                        console.log("getGames: ");
                        console.log(error);
                    }
                });
            });
        }

        getGameInfo();
    return () => {
        // Clean up the SignalR connection when the component is unmounted

        if (connection) {
            // Leave the group associated with the game
            connection.invoke('LeaveGameGroup', gameCode);
            // Stop the SignalR connection
            connection.stop();
            console.log('SignalR connection stopped.')
        }
    };
    }, []);

    // Send move to server
    const sendMoveToServer = (origin: [number, number], destination: [number, number]) => {
        if (connection && connection.state === "Connected") {
            // Send a message to the server with the origin and destination coordinates
            connection.send("MovePiece", gameCode, getUserIdFromToken(), origin[0], origin[1], destination[0], destination[1])
                .then(() => {
                    // console.log("Move sent to server:", origin, destination);
                    setOrigin(null);
                    setDestination(null);
                })
                .catch((error) => {
                    console.error("Error sending move to server:", error);
                });
        } else {
            console.error("SignalR connection is not established or is in an invalid state.");
        }
    };

    // Handle piece move
    const handlePieceMove = (row: number, col: number) => () => {
        // If origin is null, select the piece
        if (origin === null || destination !== null) {
            // Select the piece
            setOrigin([row, col]);
        } else {
            // Destination
            setDestination([row, col]);

            // Send move to server
            sendMoveToServer(origin, [row, col]);
        }
    }

    return (
        <section>
            <Nav />
            
            <h1 className="text-center text-3xl mt-2">Dammen</h1>

            <h2 className="text-center text-xl mb-2"><strong>{playerOne}</strong> vs. <strong>{playerTwo}</strong> </h2>

            <div className="text-center">
                <p>Current player: {currentPlayer}</p>
                <p>Player 1 remaining pieces: {PlayerOneRemainingPieces}</p>
                <p>Player 2 remaining pieces: {PlayerTwoRemainingPieces}</p>
                <p>Game message: {GameMessage}</p>
            </div>
            
            <CheckersBoard pieces={pieces} handleClick={handlePieceMove} />

            <div className="flex justify-center mt-4">
                <Button type="submit" id="toDashboard" value="Naar Dashboard" customClass="!max-w-48" submitAction={() => { router.push('/dashboard') }} />
            </div>
        </section>
    );
}