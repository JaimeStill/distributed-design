import {
    Component,
    Input
} from '@angular/core';

import { Package } from '@workflows/contracts';

@Component({
    selector: 'package-display',
    templateUrl: 'package-display.component.html'
})
export class PackageDisplayComponent {
    @Input() package: Package | null = null;
    @Input() cardStyle: string = 'm4 p4 rounded border-divider';
}