import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

const createHubConnection = (hubUrl: string, token: string): HubConnection => {
    console.log('Creating SignalR connection to', hubUrl, 'with token', token);
    const connection: HubConnection = new HubConnectionBuilder()
        .withUrl(hubUrl, { accessTokenFactory: () => token })
        .build();
    return connection;
};

export default createHubConnection;