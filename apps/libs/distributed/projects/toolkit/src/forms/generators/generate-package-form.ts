import {
    FormBuilder,
    FormGroup,
    Validators
} from '@angular/forms';

import {
    Package,
    PackageStates
} from '@workflows/contracts';

import { GenerateEntityForm } from './generate-entity-form';

export function GeneratePackageForm(pkg: Package, fb: FormBuilder): FormGroup {
    return fb.group({
        ...(GenerateEntityForm(pkg, fb)).controls,
        state: [pkg.state ?? PackageStates.Pending],
        result: [pkg.result, Validators.required],
        entityId: [pkg.entityId, [
            Validators.required,
            Validators.min(1)
        ]],
        entityType: [pkg.entityType, Validators.required],
        title: [pkg.title ?? '', Validators.required]
    })
}