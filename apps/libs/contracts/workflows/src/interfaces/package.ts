import {
    Intents,
    Statuses
} from '@distributed/contracts';

import { Entity } from '@distributed/core';
import { PackageStates } from '../enums';

export interface Package extends Entity {
    state: PackageStates;
    intent: Intents;
    result: Statuses;
    entityId: number;
    entityType: string;
    title: string;
}