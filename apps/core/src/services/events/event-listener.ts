import { Entity } from '../../entities';
import { EventAction } from './event-action';
import { EventClient } from './event-client';

export abstract class EventListener<T extends Entity> extends EventClient<T>{
    add: EventAction;
    update: EventAction
    remove: EventAction;

    constructor(endpoint: string) {
        super(endpoint);

        this.add = new EventAction('add', this.connection);
        this.update = new EventAction('update', this.connection);
        this.remove = new EventAction('remove', this.connection);
    }
}