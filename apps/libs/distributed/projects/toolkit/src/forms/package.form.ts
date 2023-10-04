import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Statuses } from '@distributed/contracts';
import { Package } from '@workflows/contracts';
import { StorageForm } from './storage-form';
import { WorkflowsGateway } from '../gateways';
import { GeneratePackageForm } from './generators';

@Component({
    selector: 'package-form',
    templateUrl: 'package.form.html',
    providers: [ WorkflowsGateway ]
})
export class PackageForm extends StorageForm<Package> {
    statuses: Statuses[];

    constructor(
        protected formBuilder: FormBuilder,
        protected workflowsGateway: WorkflowsGateway
    ) {
        super(formBuilder, GeneratePackageForm, workflowsGateway);

        this.statuses = Object.values(Statuses);
    }

    get entityId() { return this.form?.get('entityId') }
    get entityType() { return this.form?.get('entityType') }
    get title() { return this.form?.get('title') }
    get value() { return this.form?.get('value') }
    get result() { return this.form?.get('result') }
}