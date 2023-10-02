import {
    Component,
    Input
} from '@angular/core';

import { Proposal } from '../models';

@Component({
    selector: 'proposal-display',
    templateUrl: 'proposal-display.component.html'
})
export class ProposalDisplayComponent {
    @Input() proposal: Proposal;
    @Input() cardStyle: string = 'rounded';
}