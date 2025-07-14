interface FormInputProps {
    type: "text" | "password" | "number" | undefined;
    id: string;
    name: string;
    label: string;
    customClass?: string;
    onKeyUpCapture?: React.KeyboardEventHandler<HTMLInputElement>;
}

const FormInput: React.FC<FormInputProps> = (props) => {
    return(
        <div className={"relative z-0 w-full mb-5 group " + props.customClass}>
            <input onKeyUpCapture={props.onKeyUpCapture} type={props.type} name={props.name} id={props.id} className="block py-2.5 px-0 w-full text-sm text-paragraph bg-transparent border-0 border-b-2 border-gray-300 appearance-none focus:outline-none focus:ring-0 focus:border-blue-600 peer" placeholder=" "/>
            <label htmlFor={props.name} id={props.id + "_label"} className="peer-focus:font-medium absolute text-sm text-paragraph duration-300 transform -translate-y-6 scale-75 top-3 -z-10 origin-[0] peer-focus:start-0 peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0 peer-focus:scale-75 peer-focus:-translate-y-6">{props.label}</label>
        </div>
    );
}

export default FormInput;
