import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EntityQuery } from '@distributed/core';
import { SnackerService } from '@distributed/toolkit';
import { Package } from '@workflows/contracts';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable()
export class PackageQuery extends EntityQuery<Package> {
    constructor(
        http: HttpClient,
        snacker: SnackerService
    ) {
        super(`${environment.api}package/`, http, snacker);
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