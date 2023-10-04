import {
    FormBuilder,
    FormGroup,
    Validators
} from '@angular/forms';

import { Entity } from '@distributed/core';
import { GenerateEntityForm } from '@distributed/toolkit';


export interface Proposal extends Entity {
    statusId: number;
    packageId?: number;
    title: string;
}

export function GenerateProposalForm(proposal: Proposal, fb: FormBuilder): FormGroup {
    return fb.group({
        ...(GenerateEntityForm(proposal, fb)).controls,
        statusId: [proposal?.statusId ?? 0],
        packageId: [proposal?.packageId],
        title: [proposal?.title ?? '', Validators.required]
    })
}