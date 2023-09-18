import { Entity } from '@distributed/core';
import { Statuses } from '../enums';

export interface Status extends Entity {
    state: Statuses;
    entityId: number;
    entityType: string;
    context: string;
}