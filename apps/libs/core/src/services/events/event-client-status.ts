import { HubConnectionState } from '@microsoft/signalr';

export interface EventClientStatus {
    connectionId: string;
    state: HubConnectionState;
}