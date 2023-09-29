import { Injectable } from '@angular/core';
import { EventListener } from '@distributed/core';
import { Proposal } from '../models';
import { environment } from '../../environments/environment';

@Injectable()
export class ProposalListener extends EventListener<Proposal> {
    constructor() {
        super(`${environment.events}proposal/`);
    }
}
