import type { NextRequest } from 'next/server'
import { parseJwtToken } from './services/tokenService';

export function middleware(request: NextRequest) {
    // Get the current user from the cookies
    const currentUser = request.cookies.get('accessToken');
    
    // Redirect to the dashboard page if the user is logged in
    if (currentUser && request.nextUrl.pathname.startsWith('/account') && !request.nextUrl.pathname.startsWith('/account/mfa-setup')) {
        return Response.redirect(new URL('/dashboard', request.url))
    }
    
    // Redirect to the account page if the user is not logged in
    if (!currentUser && !request.nextUrl.pathname.endsWith('/account') ) {
        return Response.redirect(new URL('/account', request.url))
    }

    // If the user is not a beheerder, redirect to the dashboard
    let userRole;
    if (currentUser) {
        const token = parseJwtToken(currentUser.value);
        userRole = token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        // Redirect to the dashboard page if the user is not a beheerder
        if(currentUser && request.nextUrl.pathname.startsWith('/beheer') && userRole != "Beheerder"){
            return Response.redirect(new URL('/dashboard', request.url))
        }
    }
}

export const config = {
    matcher: ['/((?!api|_next/static|_next/image|.*\\.png$).*)'],
}
