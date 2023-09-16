import { HubConnection } from '@microsoft/signalr';

export class EventAction {
    constructor(
        private method: string,
        private client: HubConnection
    ) { }

    set = (method: (...args: any[]) => any) => {
        this.client.on(this.method, method);
    }
}