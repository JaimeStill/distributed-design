import {
    Inject,
    Injectable
} from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { EntityQuery } from '@distributed/core';
import { Package } from '@workflows/contracts';
import { firstValueFrom } from 'rxjs';
import { SnackerService } from '../services';

@Injectable()
export class PackageQuery extends EntityQuery<Package> {
    constructor(
        @Inject('packageApiUrl') endpoint: string,
        http: HttpClient,
        snacker: SnackerService
    ) {
        super(endpoint, http, snacker);
    }

    protected getByType$ = (entityType: string) =>
        this.http.get<Package[]>(`${this.api}getByType/${entityType}`);

    protected getByEntity$ = (id: number, entityType: string) =>
        this.http.get<Package[]>(`${this.api}getByEntity/${id}/${entityType}`);

    protected getActivePackage$ = (id: number, entityType: string) =>
        this.http.get<Package>(`${this.api}getActivePackage/${id}/${entityType}`);

    getByType = (entityType: string) =>
        firstValueFrom(this.getByType$(entityType));

    getByEntity = (id: number, entityType: string) =>
        firstValueFrom(
            this.getByEntity$(id, entityType)
        );

    getActivePackage = (id: number, entityType: string) =>
        firstValueFrom(
            this.getActivePackage$(id, entityType)
        );
}