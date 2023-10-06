
import {
  Component,
  OnInit
} from '@angular/core';

import {
  PackageListener,
  PackageQuery,
  SnackerService
} from '@distributed/toolkit';

import { EventMessage } from '@distributed/core';
import { Package } from '@workflows/contracts';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [
    PackageListener,
    PackageQuery
  ]
})
export class HomeRoute implements OnInit {
  packages: Package[] | null;

  constructor(
    private packageQuery: PackageQuery,
    public packageListener: PackageListener,
    private snacker: SnackerService
  ) { }

  private refresh = async () =>
    this.packages = await this.packageQuery.get();

  private snack = (message: string) => this.snacker.sendSuccessMessage(message);

  private sync = (event: EventMessage<Package>) => {
    this.snack(event.message);
    this.refresh();
  }

  async ngOnInit(): Promise<void> {
    this.refresh();
    await this.packageListener.connect();

    this.packageListener.onAdd.set(this.sync);
    this.packageListener.onUpdate.set(this.sync);
    this.packageListener.onRemove.set(this.sync);
    this.packageListener.onStateChanged.set(this.sync);
  }
}
