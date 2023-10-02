import {
    firstValueFrom,
    throwError
} from 'rxjs';

import { HttpClient } from '@angular/common/http';

import {
    IStorage,
    SessionStorage
} from '../storage';

import { Entity } from '../entities';
import { ApiService } from './api-service';
import { ISnacker } from './i-snacker';
import { ValidationMessage } from '../messages';
import { Strings } from '../utils';

export abstract class EntityCommand<T extends Entity> extends ApiService {
    protected storagepoint: string;

    constructor(
        protected api: string,
        http: HttpClient,
        snacker: ISnacker
    ) {
        super(http, snacker);
        this.storagepoint = Strings.dasherize(this.api);
    }

    protected onSave?(data: T): void;
    protected onRemove?(data: T): void;

    protected handleError = (err: any) => {
        this.snacker.sendErrorMessage(err.error);
        return throwError(() => new Error(err));
    }

    generateStorage(entity: T): IStorage<T> {
        return new SessionStorage<T>(
            entity.id
                ? `${this.storagepoint}-${entity.id}`
                : `${this.storagepoint}-new`
        )
    }

    validate = (entity: T): Promise<ValidationMessage> =>
        firstValueFrom(
            this.http.post<ValidationMessage>(`${this.api}validate`, entity)
        );

    async save(entity: T): Promise<T> {
        const value: T = await this.apiPost(`${this.api}save`, entity);

        if (this.onSave)
            this.onSave(value);

        return value;
    }

    async remove(entity: T): Promise<number> {
        const value: number = await this.apiRemove(`${this.api}remove`, entity);

        if (this.onRemove)
            this.onRemove(entity);

        return value;
    }
}