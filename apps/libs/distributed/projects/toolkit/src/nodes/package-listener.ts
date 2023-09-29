import {
    Inject,
    Injectable
} from '@angular/core';

import {
    EventAction,
    EventListener
} from '@distributed/core';

import { Package } from '@workflows/contracts';

@Injectable()
export class PackageListener extends EventListener<Package> {
    onStateChanged: EventAction;

    constructor(@Inject('packageEventsUrl') endpoint: string) {
        super(endpoint);

        this.onStateChanged = new EventAction('onStateChanged', this.connection);
    }
}