
import {
  Component,
  OnInit
} from '@angular/core';

import {
  PackageListener,
  PackageQuery
} from '../../services';

import { SnackerService } from '@distributed/toolkit';
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
    private packageListener: PackageListener,
    private snacker: SnackerService
  ) { }

  private refresh = async () =>
    this.packages = await this.packageQuery.get();

  private snack = () => this.snacker.sendSuccessMessage('Data synchronized');

  private sync = () => {
    this.snack();
    this.refresh();
  }

  async ngOnInit(): Promise<void> {
    this.refresh();
    await this.packageListener.connect();

    this.packageListener.add.set(this.sync);
    this.packageListener.update.set(this.sync);
    this.packageListener.remove.set(this.sync);
  }
}
