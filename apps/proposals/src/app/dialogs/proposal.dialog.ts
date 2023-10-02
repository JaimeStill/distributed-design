import {
    Component,
    Inject,
    OnInit
} from '@angular/core';

import {
    MatDialogRef,
    MAT_DIALOG_DATA
} from '@angular/material/dialog';

import { Proposal } from '../models';

@Component({
    selector: 'proposal-dialog',
    templateUrl: 'proposal.dialog.html'
})
export class ProposalDialog implements OnInit {
    constructor(
        private dialog: MatDialogRef<ProposalDialog>,
        @Inject(MAT_DIALOG_DATA) public proposal: Proposal
    ) { }

    ngOnInit(): void {
        if (!this.proposal)
            this.dialog.close();
    }

    saved = (proposal: Proposal) => this.dialog.close(proposal);
}