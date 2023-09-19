import { Entity } from '@distributed/core';

export interface ProcessTemplate extends Entity {
    workflowTemplateId: number;
    index: number;
    description: string;
}