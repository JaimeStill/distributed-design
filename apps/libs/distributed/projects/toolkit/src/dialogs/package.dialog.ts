import {
    Component,
    Inject,
    OnInit
} from '@angular/core';

import {
    MatDialogRef,
    MAT_DIALOG_DATA
} from '@angular/material/dialog';

import { Package } from '@workflows/contracts';

@Component({
    selector: 'package-dialog',
    templateUrl: 'package.dialog.html'
})
export class PackageDialog implements OnInit {
    constructor(
        private dialog: MatDialogRef<PackageDialog>,
        @Inject(MAT_DIALOG_DATA) public pkg: Package
    ) { }

    ngOnInit(): void {
        if (!this.pkg)
            this.dialog.close();
    }

    saved = (pkg: Package) => this.dialog.close(pkg);
}