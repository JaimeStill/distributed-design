import {
    FormBuilder,
    FormGroup,
    Validators
} from '@angular/forms';

import { Entity } from '@distributed/core';

export function GenerateEntityForm<T extends Entity>(e: T, fb: FormBuilder): FormGroup {
    return fb.group({
        id: [e?.id ?? 0],
        value: [e?.value ?? '', Validators.required]
    })
}