import {
    Inject,
    Injectable
} from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { EntityCommand } from '@distributed/core';
import { Package } from '@workflows/contracts';
import { SnackerService } from '../services';

@Injectable()
export class PackageCommand extends EntityCommand<Package> {
    constructor(
        @Inject('packageApiUrl') endpoint: string,
        http: HttpClient,
        snacker: SnackerService
    ) {
        super(endpoint, http, snacker);
    }

    approve = async (pkg: Package): Promise<Package> =>
        await this.apiPost(`${this.api}approve`, pkg);

    return = async (pkg: Package): Promise<Package> =>
        await this.apiPost(`${this.api}return`, pkg);

    reject = async (pkg: Package): Promise<Package> =>
        await this.apiPost(`${this.api}reject`, pkg);
}