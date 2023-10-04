import {
    Inject,
    Injectable
} from '@angular/core';

import {
    ApiService,
    ValidationMessage
} from '@distributed/core';

import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Package } from '@workflows/contracts';
import { SnackerService } from '../services';

@Injectable()
export class WorkflowsGateway extends ApiService {    
    constructor(
        @Inject('workflowsGatewayUrl') private endpoint: string,
        http: HttpClient,
        snacker: SnackerService
    )
    {
        if (!endpoint.endsWith('/'))
            endpoint = `${endpoint}/`;

        super(http, snacker);
    }

    protected getPackagesByType$ = (entityType: string) =>
        this.http.get<Package[]>(`${this.endpoint}getPackagesByType/${entityType}`);

    protected getPackagesByEntity$ = (id: number, entityType: string) =>
        this.http.get<Package[]>(`${this.endpoint}getPackagesByEntity/${id}/${entityType}`);

    protected getActivePackage$ = (id: number, entityType: string) =>
        this.http.get<Package>(`${this.endpoint}getActivePackage/${id}/${entityType}`);

    getPackagesByType(entityType: string) {
        return firstValueFrom(
            this.getPackagesByType$(entityType)
        );
    }

    getPackagesByEntity(id: number, entityType: string) {
        return firstValueFrom(
            this.getPackagesByEntity$(id, entityType)
        );
    }

    getActivePackage(id: number, entityType: string) {
        return firstValueFrom(
            this.getActivePackage$(id, entityType)
        );
    }

    validatePackage = async (pkg: Package): Promise<ValidationMessage> =>
        await this.apiPost(`${this.endpoint}validatePackage`, pkg);

    submitPackage = async (pkg: Package): Promise<Package> =>
        await this.apiPost(`${this.endpoint}submitPackage`, pkg);

    withdrawPackage = async (pkg: Package): Promise<number> =>
        await this.apiPost(`${this.endpoint}withdrawPackage`, pkg);
}