import {
    Component,
    EventEmitter,
    Input,
    Output
} from '@angular/core';

import { TooltipPosition } from '@angular/material/tooltip';
import { Proposal } from '../models';

@Component({
    selector: 'proposal-card',
    templateUrl: 'proposal-card.component.html'
})
export class ProposalCardComponent {
    @Input() proposal: Proposal;
    @Input() size: number | string = 360;
    @Input() cardStyle: string = 'm4 rounded border-divider background-card';
    @Input() tooltipLocation: TooltipPosition = 'below';

    @Input() showActions: boolean = true;
    @Input() showDetails: boolean = true;

    @Output() edit = new EventEmitter<Proposal>();
    @Output() submit = new EventEmitter<Proposal>();
    @Output() remove = new EventEmitter<Proposal>();
}