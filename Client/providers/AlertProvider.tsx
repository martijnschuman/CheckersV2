"use client"
import Alert from '@/components/misc/Alert'
import React, { createContext, useState, useContext } from 'react';


// Define the context
const AlertContext = createContext<any>(null);

// Define the provider
export const AlertProvider = ({ children }: { children: React.ReactNode }) => {
    const [alertDangerVisible, setAlertDangerVisible] = useState<[boolean, string]>([false, ""]);
    const [alertSuccessVisible, setAlertSuccessVisible] = useState<[boolean, string]>([false, ""]);

    const [dangerFirstItem, ...dangerRestItems] = alertDangerVisible;
	const showDangerAlert = dangerFirstItem && dangerFirstItem !== null;
	const dangerAlertMessage = showDangerAlert ? dangerRestItems[0] : "";

    const [successFirstItem, ...successRestItems] = alertSuccessVisible;
	const showSuccessAlert = successFirstItem && successFirstItem !== null;
	const successAlertMessage = showSuccessAlert ? successRestItems[0] : "";

    return (
        <AlertContext.Provider value={{ alertDangerVisible, setAlertDangerVisible, alertSuccessVisible, setAlertSuccessVisible }}>
            {showDangerAlert && <Alert message={dangerAlertMessage} type="danger" />}
            {showSuccessAlert && <Alert message={successAlertMessage} type="success" />}

            {children}
        </AlertContext.Provider>
    );
};

// Define a hook to use the alert context
export const useAlert = () => {
    const context = useContext(AlertContext);
    if (context === undefined) {
        throw new Error('useAlert must be used within a AlertProvider');
    }
    return context;
};