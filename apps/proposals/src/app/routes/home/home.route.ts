import {
  Component,
  OnInit
} from '@angular/core';

import {
  ConfirmDialog,
  PackageDialog,
  SnackerService,
  WorkflowsGateway
} from '@distributed/toolkit';

import { MatDialog } from '@angular/material/dialog';

import {
  ProposalCommand,
  ProposalListener,
  ProposalQuery
} from '../../services';

import { ProposalDialog } from '../../dialogs';
import { Proposal } from '../../models';
import { EventMessage } from '@distributed/core';
import { Package } from '@workflows/contracts';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [
    ProposalCommand,
    ProposalListener,
    ProposalQuery,
    WorkflowsGateway
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
    private workflowsGateway: WorkflowsGateway,
    public proposalListener: ProposalListener
  ) { }

  private refresh = async () =>
    this.proposals = await this.proposalQuery.get();

  private snack = (message: string) => this.snacker.sendSuccessMessage(message);

  private sync = (event: EventMessage<Proposal>) => {
    this.snack(event.message);
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

  private resubmitPackage = (pkg: Package) => this.dialog.open(ConfirmDialog, {
    disableClose: true,
    autoFocus: false,
    data: {
      title: 'Resubmit Package',
      content: `Are you sure you want to resubmit Package ${pkg.title}?`
    }
  })
  .afterClosed()
  .subscribe(async (result: boolean) => {
    if (result)
      await this.workflowsGateway.submitPackage(pkg);
  });

  private submitPackage = (pkg: Package) => this.dialog.open(PackageDialog, {
    ...this.dialogOptions,
    data: pkg
  });

  create = () => this.openProposalDialog(<Proposal>{});

  edit = (proposal: Proposal) => this.openProposalDialog(proposal);

  submit = (pkg: Package) => pkg.id > 0
    ? this.resubmitPackage(pkg)
    : this.submitPackage(pkg);

  withdraw = (pkg: Package) => this.dialog.open(ConfirmDialog, {
    disableClose: true,
    autoFocus: false,
    data: {
      title: 'Withdraw Package',
      content: `Are you sure you want to withdraw Package ${pkg.title}?`
    }
  })
  .afterClosed()
  .subscribe(async (result: boolean) => {
    if (result)
      await this.workflowsGateway.withdrawPackage(pkg);
  });

  remove = (proposal: Proposal) => this.dialog.open(ConfirmDialog, {
    disableClose: true,
    autoFocus: false,
    data: {
      title: 'Remove Proposal',
      content: `Are you sure you want to remove Proposal ${proposal.title}?`
    }
  })
  .afterClosed()
  .subscribe(async (result: boolean) => {
    if (result)
      await this.proposalCommand.remove(proposal);
  });
}
