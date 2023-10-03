import {
  Component,
  OnInit
} from '@angular/core';

import {
  ConfirmDialog,
  SnackerService
} from '@distributed/toolkit';

import { MatDialog } from '@angular/material/dialog';

import {
  ProposalCommand,
  ProposalListener,
  ProposalQuery
} from '../../services';

import { ProposalDialog } from '../../dialogs';
import { Proposal } from '../../models';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [
    ProposalCommand,
    ProposalListener,
    ProposalQuery
  ]
})
export class HomeRoute implements OnInit {
  private dialogOptions = {
    disableClose: true,
    width: '80%',
    minWidth: 420,
    maxWidth: 800
  }

  proposals: Proposal[] | null;

  constructor(
    private dialog: MatDialog,
    private proposalCommand: ProposalCommand,
    private proposalQuery: ProposalQuery,
    private snacker: SnackerService,
    public proposalListener: ProposalListener
  ) { }

  private refresh = async () =>
    this.proposals = await this.proposalQuery.get();

  private snack = () => this.snacker.sendSuccessMessage(`Data synchronized`);

  private sync = () => {
    this.snack();
    this.refresh();
  }

  async ngOnInit(): Promise<void> {
    this.refresh();
    await this.proposalListener.connect();

    this.proposalListener.onAdd.set(this.sync);
    this.proposalListener.onUpdate.set(this.sync);
    this.proposalListener.onRemove.set(this.sync);
  }

  private openProposalDialog = (proposal: Proposal) => this.dialog.open(ProposalDialog, {
    data: proposal,
    ...this.dialogOptions
  });

  create = () => this.openProposalDialog(<Proposal>{});

  edit = (proposal: Proposal) => this.openProposalDialog(proposal);

  remove = (proposal: Proposal) => this.dialog.open(ConfirmDialog, {
    disableClose: true,
    autoFocus: false,
    data: {
      title: 'Remove Proposal',
      content: `Are you sure you want to remove Proposal ${proposal.title}`
    }
  })
  .afterClosed()
  .subscribe(async (result: boolean) => {
    if (result)
      await this.proposalCommand.remove(proposal);
  });
}
