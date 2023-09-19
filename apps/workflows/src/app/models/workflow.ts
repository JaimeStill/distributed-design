import { Entity } from '@distributed/core';
import { WorkflowStates } from '@workflows/contracts';

export interface Workflow extends Entity {
    packageId: number;
    state: WorkflowStates;
    description: string;
}