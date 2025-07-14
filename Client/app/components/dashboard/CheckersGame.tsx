import React from 'react';
import { useState } from 'react';
import Button from '@/components/misc/Button';
import { getUserIdFromToken } from '@/services/tokenService';

interface GameProps {
    Id: string;
    Color: "bg-green-400" | "bg-yellow-200" | "bg-red-400" | "bg-blue-400";
    Status: "Open" | "Bezig" | "Gewonnen" | "Gelijk" | "Verloren" | "Geannuleerd" | "Speler 1 gewonnen" | "Speler 2 gewonnen";
    GameCode?: number;
    StonesWhite: number;
    StonesBlack: number;
    PlayerOne: string;
    PlayerOneId: string;
    PlayerTwo: string;
    PlayerTwoId: string;
    StartDateTime: string;
    EndDateTime: string;

    cancelAction?: () => void;
}

const CheckersGame: React.FC<GameProps> = (props) => {
    const [showCancelButton, setShowCancelButton] = useState<boolean>(false);

    return (
        <div id={props.Id} className={"content-element relative col-span-1 md:col-span-1 shadow-lg rounded-md p-3 h-full sm-w-8/12 text-center !" + props.Color }>
            <h2 className="text-xl font-medium underline underline-offset-8 mb-1">{props.Status} {props.GameCode != null && props.Status == "Open" ? "[" + props.GameCode + "]" : ""} | wit: {props.StonesWhite} {props.StonesWhite == 1 ? "steen" : "stenen" } | zwart: {props.StonesBlack} {props.StonesBlack == 1 ? "steen" : "stenen" }</h2>
            <h2 className="text-md font-medium">{props.PlayerOne} vs. {props.PlayerTwo}</h2>
            <h2 className="text-md font-medium">{props.StartDateTime} -- {props.EndDateTime} </h2>
            
            { getUserIdFromToken() == props.PlayerOneId || getUserIdFromToken() == props.PlayerTwoId ? <Button id="play" type='submit' value='Spelen' customClass="absolute right-1 top-6 !w-24" submitAction={() => { window.location.href="/play" }}/> : ""}
            
            { props.cancelAction != null &&
                !showCancelButton ? 
                    <Button type='submit' id={"cancel_"+props.Id} value='Annuleren' customClass="absolute right-1 top-6 !w-28" onClick={() => setShowCancelButton(true)}/> 
                : showCancelButton &&
                    <div className="absolute right-1 top-6 flex float-right">
                        <Button type='submit' id={"submit_"+props.Id} value='Annuleren' customClass="!w-28 !bg-red-500" submitAction={props.cancelAction}/>
                        <Button type='submit' id={"close_"+props.Id} value='Sluiten' customClass="!w-24" onClick={() => setShowCancelButton(false)}/>
                    </div>
            }

        </div>
    );
};

export default CheckersGame;