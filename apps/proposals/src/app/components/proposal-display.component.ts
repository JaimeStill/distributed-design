import {
    Component,
    Input
} from '@angular/core';

import { Status } from '@distributed/contracts';
import { Proposal } from '../models';

@Component({
    selector: 'proposal-display',
    templateUrl: 'proposal-display.component.html'
})
export class ProposalDisplayComponent {
    @Input() proposal: Proposal;
    @Input() status: Status;
    @Input() cardStyle: string = 'rounded';
}