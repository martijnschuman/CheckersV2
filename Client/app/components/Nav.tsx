"use client";
import Cookies from 'js-cookie'
import { useRouter } from 'next/navigation';
import { faUser } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

const Nav: React.FC = () => {
    const router = useRouter()

    return(
        <div className="absolute top-2 right-2">
            <button id="dropdownDefaultButton" data-dropdown-toggle="dropdown" className="text-white bg-blue-700 hover:bg-blue-800 font-medium rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center" type="button">
                <FontAwesomeIcon icon={faUser} className="text-white" /> 
            </button>

            <div id="dropdown" className="z-10 hidden bg-white divide-y divide-gray-100 rounded-lg shadow w-44 dark:bg-gray-700" data-popper-placement="bottom">
                <ul className="py-2 text-sm text-gray-700 dark:text-gray-200" aria-labelledby="dropdownDefaultButton">
                    <li id="dashboard">
                        <a href="/dashboard" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Dashboard</a>
                    </li>
                    <li id="mfa">
                        <a href="/account/mfa-setup" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">2FA setup</a>
                    </li>
                    <li id="logout">
                        <a href="#" className="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white" onClick={() => { Cookies.remove('accessToken'); router.push('/account') }}>Uitloggen</a>
                    </li>
                </ul>
            </div>
        </div>
    );
}

export default Nav;