import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { Entity } from '../entities';
import { ApiService } from './api-service';
import { ISnacker } from './i-snacker';

export abstract class EntityQuery<T extends Entity> extends ApiService {
    constructor(
        protected api: string,
        http: HttpClient,
        snacker: ISnacker
    ) {
        super(http, snacker);
    }

    protected get$ = () =>
        this.http.get<T[]>(`${this.api}get`);

    protected getById$ = (id: number) =>
        this.http.get<T>(`${this.api}getById/${id}`);

    get = () => firstValueFrom(this.get$());

    getById = (id: number) => firstValueFrom(this.getById$(id));
}