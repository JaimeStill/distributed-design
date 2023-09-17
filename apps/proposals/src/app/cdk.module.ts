import { NgModule } from '@angular/core';

import { ClipboardModule } from '@angular/cdk/clipboard';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { LayoutModule } from '@angular/cdk/layout';

@NgModule({
    exports: [
        ClipboardModule,
        DragDropModule,
        LayoutModule
    ]
})
export class CdkModule { }