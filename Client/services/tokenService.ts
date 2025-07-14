import Cookies from 'js-cookie';

/// <summary>
/// Function to get the JWT token from the cookies
/// </summary>
export function getJWTToken(){
    let token = Cookies.get("accessToken");
    if(token){
        return token;
    } else {
        console.error("No access token found");
        return "";
    }
}

/// <summary>
/// Function to parse a JWT token
/// </summary>
export function parseJwtToken(token : string) {
    if (!token) { return; }
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace('-', '+').replace('_', '/');

    return JSON.parse(atob(base64));
}

/// <summary>
/// Function to get the user id from the JWT token
/// </summary>
export function getUserIdFromToken() {
    let userId = "";

    let accessToken = getJWTToken();

    if (accessToken) {
        let token = parseJwtToken(accessToken);
        userId = token["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    }else{
        console.error("No access token found");
    }

    return userId;
}

/// <summary>
/// Function to get the username from the JWT token
/// </summary>
export function getUsernameFromToken() {
    let username = "";

    let accessToken = getJWTToken();

    if (accessToken) {
        let token = parseJwtToken(accessToken);
        username = token["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
    }else{
        console.error("No access token found");
    }

    return username;
}

/// <summary>
/// Function to get the role from the JWT token
/// </summary>
export function getUserRoleFromToken() {
    let role = "";

    let accessToken = getJWTToken();

    if (accessToken) {
        let token = parseJwtToken(accessToken);
        role = token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    }else{
        console.error("No access token found");
    }

    return role;
}