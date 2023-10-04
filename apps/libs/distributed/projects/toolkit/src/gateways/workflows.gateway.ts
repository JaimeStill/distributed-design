import {
    Inject,
    Injectable
} from '@angular/core';

import {
    ApiService,
    IStorage,
    IStorageService,
    SessionStorage,
    Strings,
    ValidationMessage
} from '@distributed/core';

import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Package } from '@workflows/contracts';
import { SnackerService } from '../services';

@Injectable()
export class WorkflowsGateway extends ApiService implements IStorageService<Package> {    
    protected storagepoint: string;

    constructor(
        @Inject('workflowsGatewayUrl') private endpoint: string,
        http: HttpClient,
        snacker: SnackerService
    )
    {
        super(http, snacker);

        if (!endpoint.endsWith('/'))
            endpoint = `${endpoint}/`;

        this.storagepoint = Strings.dasherize(endpoint);
    }

    protected getPackagesByType$ = (entityType: string) =>
        this.http.get<Package[]>(`${this.endpoint}getPackagesByType/${entityType}`);

    protected getPackagesByEntity$ = (id: number, entityType: string) =>
        this.http.get<Package[]>(`${this.endpoint}getPackagesByEntity/${id}/${entityType}`);

    protected getActivePackage$ = (id: number, entityType: string) =>
        this.http.get<Package>(`${this.endpoint}getActivePackage/${id}/${entityType}`);

    generateStorage(pkg: Package): IStorage<Package> {
        return new SessionStorage<Package>(
            pkg.id
                ? `${this.storagepoint}-${pkg.id}`
                : `${this.storagepoint}-new`
        );
    }

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

    save = this.submitPackage;
}