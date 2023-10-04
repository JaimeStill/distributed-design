import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { StorageForm } from '@distributed/toolkit';
import { ProposalCommand } from '../services';
import { GenerateProposalForm, Proposal } from '../models';

@Component({
    selector: 'proposal-form',
    templateUrl: 'proposal.form.html',
    providers: [ ProposalCommand ]
})
export class ProposalForm extends StorageForm<Proposal> {
    constructor(
        protected formBuilder: FormBuilder,
        protected proposalCommand: ProposalCommand
    ) {
        super(formBuilder, GenerateProposalForm, proposalCommand);
    }

    get title() { return this.form?.get('title') }
    get value() { return this.form?.get('value') }
}