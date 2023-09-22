import { Entity } from '../../entities';
import { EventAction } from './event-action';
import { EventClient } from './event-client';

export abstract class EventListener<T extends Entity> extends EventClient<T>{
    onAdd: EventAction;
    onUpdate: EventAction
    onRemove: EventAction;

    constructor(endpoint: string) {
        super(endpoint);

        this.onAdd = new EventAction('onAdd', this.connection);
        this.onUpdate = new EventAction('onUpdate', this.connection);
        this.onRemove = new EventAction('onRemove', this.connection);
    }
}