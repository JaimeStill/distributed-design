import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Status } from '@distributed/contracts';
import { EntityQuery } from '@distributed/core';
import { SnackerService } from '@distributed/toolkit';
import { firstValueFrom } from 'rxjs';
import { Proposal } from '../models';
import { environment } from '../../environments/environment';

@Injectable()
export class ProposalQuery extends EntityQuery<Proposal> {
    constructor(
        http: HttpClient,
        snacker: SnackerService
    ) {
        super(`${environment.api}proposal/`, http, snacker);
    }

    protected getStatus$ = (id: number) =>
        this.http.get<Status>(`${this.api}getStatus/${id}`);

    getStatus = (id: number) =>
        firstValueFrom(
            this.getStatus$(id)
        );
}
