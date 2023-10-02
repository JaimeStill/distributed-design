import { IStorage } from './istorage';

export class SessionStorage<T> implements IStorage<T> {
    constructor(private key: string) { }

    hasState = (): boolean => sessionStorage.hasOwnProperty(this.key);

    get(): T | null {
        const item: string | null = sessionStorage.getItem(this.key);

        if (item === null)
            return item;

        return <T>JSON.parse(item);
    }

    set(value: T | null): void {
        value
            ? sessionStorage.setItem(this.key, JSON.stringify(value))
            : this.clear();
    }

    clear(): void {
        sessionStorage.removeItem(this.key);
    }
}