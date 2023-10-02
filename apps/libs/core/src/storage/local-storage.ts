import { IStorage } from './istorage';

export class LocalStorage<T> implements IStorage<T> {
    constructor(private key: string) { }

    hasState = (): boolean => localStorage.hasOwnProperty(this.key);

    get(): T | null {
        const item: string | null = localStorage.getItem(this.key);

        if (item === null)
            return item;

        return <T>JSON.parse(item);
    }

    set(value: T | null): void {
        value
            ? localStorage.setItem(this.key, JSON.stringify(value))
            : this.clear();
    }

    clear(): void {
        localStorage.removeItem(this.key);
    }
}