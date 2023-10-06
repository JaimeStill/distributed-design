import {
    Component,
    EventEmitter,
    Input,
    Output
} from '@angular/core';

import {
    Package,
    PackageStates
} from '@workflows/contracts';

import { TooltipPosition } from '@angular/material/tooltip';

@Component({
    selector: 'package-card',
    templateUrl: 'package-card.component.html'
})
export class PackageCardComponent {
    @Input() package: Package;
    @Input() size: number | string = 360;
    @Input() cardStyle: string = 'm4 rounded border-divider background-card';
    @Input() tooltipLocation: TooltipPosition = 'below';
    @Input() showActions: boolean = true;

    @Output() approve = new EventEmitter<Package>();
    @Output() return = new EventEmitter<Package>();
    @Output() reject = new EventEmitter<Package>();

    actionsAvailable = (): boolean =>
        this.showActions
        && this.package?.state === PackageStates.Pending;
}