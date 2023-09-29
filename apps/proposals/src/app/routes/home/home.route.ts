import {
  Component,
  OnInit
} from '@angular/core';

import {
  ProposalListener,
  ProposalQuery
} from '../../services';

import { SnackerService } from '@distributed/toolkit';
import { Proposal } from '../../models';

@Component({
  selector: 'home-route',
  templateUrl: 'home.route.html',
  providers: [
    ProposalListener,
    ProposalQuery
  ]
})
export class HomeRoute implements OnInit {
  proposals: Proposal[] | null;

  constructor(
    private proposalQuery: ProposalQuery,
    public proposalListener: ProposalListener,
    private snacker: SnackerService
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
}
