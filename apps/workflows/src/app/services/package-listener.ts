import { Injectable } from '@angular/core';
import { EventAction, EventListener } from '@distributed/core';
import { Package } from '@workflows/contracts';
import { environment } from '../../environments/environment';

@Injectable()
export class PackageListener extends EventListener<Package> {
    onComplete: EventAction;

    constructor() {
        super(`${environment.events}package/`);

        this.onComplete = new EventAction('onComplete', this.connection);
    }
}