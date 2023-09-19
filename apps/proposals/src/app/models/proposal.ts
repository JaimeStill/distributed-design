import { Entity } from '@distributed/core';

export interface Proposal extends Entity {
    statusId: number;
    packageId?: number;
    title: string;
}