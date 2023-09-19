import { Entity } from '@distributed/core';
import { ProcessActions } from '@workflows/contracts';

export interface Process extends Entity {
    workflowId: number;
    index: number;
    action?: ProcessActions;
    description: string;
}