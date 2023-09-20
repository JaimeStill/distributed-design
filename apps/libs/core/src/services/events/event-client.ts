import {
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel
} from '@microsoft/signalr';

import { EventAction } from './event-action';
import { EventClientStatus } from './event-client-status';
import { EventMessage } from './event-message';

export abstract class EventClient<T> {
    protected connection!: HubConnection;
    protected endpoint!: string;

    get status(): EventClientStatus {
        return <EventClientStatus>{
            connectionId: this.connection.connectionId,
            state: this.connection.state
        }
    }

    onPing!: EventAction;
    onSync!: EventAction;

    protected buildHubConnection = (endpoint: string): HubConnection =>
        new HubConnectionBuilder()
            .withUrl(endpoint)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

    protected initialize = (): void => {
        this.connection.onclose(async () => await this.connect());
        this.onPing = new EventAction("ping", this.connection);
        this.onSync = new EventAction("sync", this.connection);

        this.onPing.set(() => console.log('Pong'));
    }

    constructor(endpoint: string) {
        this.endpoint = endpoint;

        this.connection = this.buildHubConnection(endpoint);
        this.initialize();
    }

    async connect() {
        if (this.connection.state !== HubConnectionState.Connected) {
            while (true) {
                try {
                    await this.connection.start();
                } catch {
                    setTimeout(this.connect, 5000);
                }
            }
        }
    }

    async ping() {
        await this.connection.invoke('ping');
    }

    async sync(message: EventMessage<T>) {
        await this.connection.invoke('sync', message);
    }
}