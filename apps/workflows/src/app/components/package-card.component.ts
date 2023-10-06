import {
    Component,
    EventEmitter,
    Input,
    Output
} from '@angular/core';

import { TooltipPosition } from '@angular/material/tooltip';
import { Package } from '@workflows/contracts';

@Component({
    selector: 'package-card',
    templateUrl: 'package-card.component.html'
})
export class PackageCardComponent {
    @Input() package: Package;
    @Input() size: number | string = 360;
    @Input() cardStyle: string = 'm4 rounded border-divider background-card';
    @Input() tooltipLocation: TooltipPosition = 'below';

    @Output() approve = new EventEmitter<Package>();
    @Output() return = new EventEmitter<Package>();
    @Output() reject = new EventEmitter<Package>();
}