import {
    Intents,
    Statuses
} from '@distributed/contracts';

import { Entity } from '@distributed/core';
import { WorkflowStates } from '../enums';

export interface Package extends Entity {
    workflowId: number | null;
    state: WorkflowStates;
    intent: Intents;
    result: Statuses;
    entityId: number;
    entityType: string;
    context: string;
    title: string;
}