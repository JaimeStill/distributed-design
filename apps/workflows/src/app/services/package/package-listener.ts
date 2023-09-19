import { Injectable } from '@angular/core';
import { EventAction, EventListener } from '@distributed/core';
import { Package } from '@workflows/contracts';
import { environment } from '../../../environments/environment';

@Injectable()
export class PackageListener extends EventListener<Package> {
    complete: EventAction;

    constructor() {
        super(`${environment.events}package/`);

        this.complete = new EventAction('complete', this.connection);
    }
}