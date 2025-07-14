import { faCrown } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface CheckersPieceProps {
    checkerPieceId: number;
    owner: string;
    playerNumber: number;
    isKing: boolean;
}

const CheckersPiece: React.FC<CheckersPieceProps> = (props) => {
    return (
        <div className={`piece text-center m-auto`} id={props.checkerPieceId.toString()}> 
            <div className={"rounded-3xl w-8 h-8 " + (props.playerNumber === 1 ? "piece-player1" : "piece-player2")}>
                {props.isKing ? <FontAwesomeIcon icon={faCrown} height={25} className="mt-2" /> : ""}
            </div> 
        </div>
    );
}

export default CheckersPiece;
