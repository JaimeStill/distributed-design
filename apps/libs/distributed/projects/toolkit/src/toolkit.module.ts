import { NgModule } from '@angular/core';
import { CdkModule } from './cdk.module';
import { MaterialModule } from './material.module';

import { Dialogs } from './dialogs';
import { Directives } from './directives';
import { Pipes } from './pipes';



@NgModule({
  declarations: [
    ...Dialogs,
    ...Directives,
    ...Pipes
  ],
  imports: [
    CdkModule,
    MaterialModule
  ],
  exports: [
    ...Dialogs,
    ...Directives,
    ...Pipes
  ]
})
export class ToolkitModule { }
