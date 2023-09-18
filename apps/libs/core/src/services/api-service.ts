import { HttpClient } from '@angular/common/http';
import { ApiMessage } from '../messages';
import { ISnacker } from './i-snacker';
import { Observer } from 'rxjs';

export abstract class ApiService {
    constructor(
        protected http: HttpClient,
        protected snacker: ISnacker
    ) { }

    protected handleApiResult<T>(
        resolve: (value: T | PromiseLike<T>) => void,
        reject: (reason?: any) => void
    ) : Partial<Observer<ApiMessage<T>>> {
        return <Partial<Observer<ApiMessage<T>>>> {
            next: (result: ApiMessage<T>) => {
                if (result.error) {
                    this.snacker.sendErrorMessage(result.message);
                    reject(result);
                } else {
                    this.snacker.sendSuccessMessage(result.message);
                    resolve(result.data!);
                }
            },
            error: (err: any) => {
                this.snacker.sendErrorMessage(err?.error ?? err);
                reject(err);
            }
        }
    }

    protected apiGet<R>(endpoint: string): Promise<R> {
        return new Promise((resolve, reject) => {
            this.http.get<ApiMessage<R>>(endpoint)
                .subscribe(this.handleApiResult(resolve, reject));
        });
    }

    protected apiPost<T, R>(endpoint: string, data: T): Promise<R> {
        return new Promise((resolve, reject) => {
            this.http.post<ApiMessage<R>>(endpoint, data)
                .subscribe(this.handleApiResult(resolve, reject));
        });
    }

    protected apiRemove<T, R>(endpoint: string, data?: T): Promise<R> {
        return new Promise((resolve, reject) => {
            this.http.delete<ApiMessage<R>>(endpoint, { body: data })
                .subscribe(this.handleApiResult(resolve, reject));
        });
    }
}