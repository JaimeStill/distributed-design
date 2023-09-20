import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EntityCommand } from '@distributed/core';
import { SnackerService } from '@distributed/toolkit';
import { Package } from '@workflows/contracts';
import { environment } from '../../../environments/environment';

@Injectable()
export class PackageCommand extends EntityCommand<Package> {
    constructor(
        http: HttpClient,
        snacker: SnackerService
    ) {
        super(`${environment.api}package/`, http, snacker);
    }
}