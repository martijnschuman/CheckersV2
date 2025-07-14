interface AlertProps {
    message: string;
    type: "success" | "info" | "danger";
}

const Alert: React.FC<AlertProps> = (props) => {
    const alertStyles = {
        success: "text-green-800 border-green-300 bg-green-50",
        info: "text-blue-800 border-blue-300 bg-blue-50",
        danger: "text-red-800 border-red-300 bg-red-50"
    };

    return (
        <section className='fixed top-10 sm:right-3 sm:top-3 animate__animated animate__fadeInDown'>
            <div id="alert-border-2" className={"flex items-center p-4 mb-4 border-t-4 rounded-md md:w-96 " + alertStyles[props.type ?? "info"] } role="alert">
                <svg className="flex-shrink-0 w-4 h-4" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 .5a9.5 9.5 0 1 0 9.5 9.5A9.51 9.51 0 0 0 10 .5ZM9.5 4a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3ZM12 15H8a1 1 0 0 1 0-2h1v-3H8a1 1 0 0 1 0-2h2a1 1 0 0 1 1 1v4h1a1 1 0 0 1 0 2Z"/>
                </svg>
                <div className="ms-3 text-sm font-medium">
                    {props.message}
                </div>
            </div>
        </section>
    );
};

export default Alert;