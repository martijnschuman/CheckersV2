"use client"
import React, { useState } from 'react';
import AccountSignIn from "@/components/account/AccountSignIn";
import AccountSignUp from "@/components/account/AccountSignUp";


export default function Page() {
    const [showSignIn, setShowSignIn] = useState<boolean>(true);

	return (
        <div className="h-screen flex items-center justify-center">
            {showSignIn ? <AccountSignIn setShowSignIn={setShowSignIn} /> : <AccountSignUp setShowSignIn={setShowSignIn}/>}
        </div>
    )
}