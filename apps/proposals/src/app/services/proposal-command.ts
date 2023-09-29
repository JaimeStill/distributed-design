import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EntityCommand } from '@distributed/core';
import { SnackerService } from '@distributed/toolkit';
import { Proposal } from '../models';
import { environment } from '../../environments/environment';

@Injectable()
export class ProposalCommand extends EntityCommand<Proposal> {
    constructor(
        http: HttpClient,
        snacker: SnackerService
    ) {
        super(`${environment.api}proposal/`, http, snacker);
    }
}