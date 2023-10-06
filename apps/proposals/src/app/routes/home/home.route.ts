import {
  Component,
  OnInit
} from '@angular/core';

import {
  BehaviorSubject,
  Observable
} from 'rxjs';

import {
  ConfirmDialog,
  PackageDialog,
  PackageListener,
  SnackerService,
  WorkflowsGateway
} from '@distributed/toolkit';

import { MatDialog } from '@angular/material/dialog';

import {
  ProposalCommand,
  ProposalListener,
  ProposalQuery
} from '../../services';

import { EventMessage } from '@distributed/core';
import { Package } from '@workflows/contracts';
import { ProposalDialog } from '../../dialogs';
import { Proposal } from '../../models';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [
    PackageListener,
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

  private trigger$ = new BehaviorSubject<EventMessage<Package>>(null);

  proposals: Proposal[] | null;
  trigger: Observable<EventMessage<Package>> = this.trigger$.asObservable();
  

  constructor(
    private dialog: MatDialog,
    private packageListener: PackageListener,
    private proposalCommand: ProposalCommand,
    private proposalQuery: ProposalQuery,
    private snacker: SnackerService,
    private workflowsGateway: WorkflowsGateway,
    public proposalListener: ProposalListener
  ) { }

  private sendTrigger = (event: EventMessage<Package>) =>
    this.trigger$.next(event);

  private refresh = async () =>
    this.proposals = await this.proposalQuery.get();

  private snack = (message: string) => this.snacker.sendSuccessMessage(message);

  private syncProposals = (event: EventMessage<Proposal>) => {
    this.snack(event.message);
    this.refresh();
  }

  async ngOnInit(): Promise<void> {
    this.refresh();
    await this.packageListener.connect();
    this.packageListener.onAdd.set(this.sendTrigger);
    this.packageListener.onUpdate.set(this.sendTrigger);
    this.packageListener.onRemove.set(this.sendTrigger);
    this.packageListener.onStateChanged.set(this.sendTrigger);

    await this.proposalListener.connect();
    this.proposalListener.onAdd.set(this.syncProposals);
    this.proposalListener.onUpdate.set(this.syncProposals);
    this.proposalListener.onRemove.set(this.syncProposals);
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
