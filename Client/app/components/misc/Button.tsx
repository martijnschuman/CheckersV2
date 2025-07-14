import React, { MouseEventHandler } from 'react';

interface ButtonProps {
    type: "submit" | "reset" | "button" | undefined;
    id: string;
    value: string;
    submitAction?: MouseEventHandler<HTMLButtonElement>;
    onClick?: () => void;
    customClass?: string;
}

const Button: React.FC<ButtonProps> = (props) => {
    return(
        <button type={props.type} id={props.id} onClick={props.submitAction || props.onClick} className={"text-white button-submit mb-3 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm w-full px-5 py-2.5 text-center " + props.customClass} >{props.value}</button>
    );
}

export default Button;