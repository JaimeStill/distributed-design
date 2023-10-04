import { Entity } from '../entities';
import { IStorage } from '../storage';

export interface IStorageService<T extends Entity> {
    save: (entity: T) => Promise<T>;
    generateStorage: (entity: T) => IStorage<T>;
}