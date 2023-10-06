
import {
  Component,
  OnInit
} from '@angular/core';

import {
  ConfirmDialog,
  PackageCommand,
  PackageListener,
  PackageQuery,
  SnackerService
} from '@distributed/toolkit';

import { MatDialog } from '@angular/material/dialog';
import { EventMessage } from '@distributed/core';
import { Package } from '@workflows/contracts';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [
    PackageCommand,
    PackageListener,
    PackageQuery
  ]
})
export class HomeRoute implements OnInit {
  packages: Package[] | null;
  archive: Package[] | null;

  constructor(
    private dialog: MatDialog,
    private packageCommand: PackageCommand,
    private packageQuery: PackageQuery,
    private snacker: SnackerService,
    public packageListener: PackageListener
  ) { }

  private async refresh() {
    this.packages = await this.packageQuery.getActive();
    this.archive = await this.packageQuery.getCompleted();
  }

  private snack = (message: string) => this.snacker.sendSuccessMessage(message);

  private sync = (event: EventMessage<Package>) => {
    this.snack(event.message);
    this.refresh();
  }

  private confirm = (pkg: Package, action: string) => this.dialog.open(ConfirmDialog, {
    data: {
      title: `${action} Package`,
      content: `Are you sure you want to ${action} Package ${pkg.title}?`
    },
    disableClose: true,
    autoFocus: false
  })
  .afterClosed();

  private handleStateAction = (pkg: Package, action: (pkg: Package) => Promise<Package>): (result: boolean) => Promise<void> =>
    async (result: boolean) => {
      if (result) {
        await action(pkg);
      }
    }

  async ngOnInit(): Promise<void> {
    this.refresh();
    await this.packageListener.connect();

    this.packageListener.onAdd.set(this.sync);
    this.packageListener.onUpdate.set(this.sync);
    this.packageListener.onRemove.set(this.sync);
    this.packageListener.onStateChanged.set(this.sync);
  }

  approve = (pkg: Package) => this.confirm(pkg, 'Approve')
    .subscribe(
      this.handleStateAction(pkg, this.packageCommand.approve)
    );

  return = (pkg: Package) => this.confirm(pkg, 'Return')
    .subscribe(
      this.handleStateAction(pkg, this.packageCommand.return)
    );

  reject = (pkg: Package) => this.confirm(pkg, 'Reject')
    .subscribe(
      this.handleStateAction(pkg, this.packageCommand.reject)
    );
}
