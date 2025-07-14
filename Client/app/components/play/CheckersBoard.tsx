import React from 'react';
import CheckersPiece from "@/components/play/CheckersPiece";

interface CheckersBoardProps {
    pieces: any[];
    handleClick: (row: number, col: number) => (e: React.MouseEvent<HTMLDivElement>) => void;
}

const CheckersBoard: React.FC<CheckersBoardProps> = (props) => {
    const boardSize = 10; // 10x10 board
    let board = [];

    for (let i = 0; i < boardSize; i++) {
        let row = [];
        for (let j = 0; j < boardSize; j++) {
            // Alternate the color of the squares
            let color = (i + j) % 2 === 0 ? 'square-white' : 'square-black';

            // Check if there is a piece at this position
            let piece = props.pieces.find(p => p.rowIndex === i && p.colIndex === j && p.isTaken === false);
            let pieceElement = piece ? (
                <CheckersPiece
                    key={piece.checkerPieceId}
                    checkerPieceId={piece.checkerPieceId}
                    owner={piece.playerNumber}
                    isKing={piece.isKing}
                    playerNumber={piece.playerNumber}
                />
            ) : null;

            row.push(
                <div key={`${i}-${j}`} className={`flex justify-center square ${color}`} onClick={props.handleClick(i, j)}>
                    {pieceElement}
                </div>
            );
        }
        board.push(<div key={i} className="row">{row}</div>);
    }

    return (
        <div className="p-4 rounded-xl checkers-board mx-auto" style={{ width: 520 }}>
            {board}
        </div>
    );
}

export default CheckersBoard;