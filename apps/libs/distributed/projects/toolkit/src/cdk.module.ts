import { NgModule } from '@angular/core';
import { OverlayModule } from '@angular/cdk/overlay';
import { TextFieldModule } from '@angular/cdk/text-field';

@NgModule({
    exports: [
        OverlayModule,
        TextFieldModule
    ]
})
export class CdkModule { }