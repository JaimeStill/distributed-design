import {
  ModuleWithProviders,
  NgModule
} from '@angular/core';

import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { CdkModule } from './cdk.module';
import { MaterialModule } from './material.module';

import { Components } from './components';
import { Dialogs } from './dialogs';
import { Directives } from './directives';
import { Pipes } from './pipes';
import { ToolkitConfig } from './toolkit.config';



@NgModule({
  declarations: [
    ...Components,
    ...Dialogs,
    ...Directives,
    ...Pipes
  ],
  imports: [
    ReactiveFormsModule,
    BrowserModule,
    CdkModule,
    MaterialModule
  ],
  exports: [
    ...Components,
    ...Dialogs,
    ...Directives,
    ...Pipes
  ]
})
export class ToolkitModule {
  static forRoot(config: ToolkitConfig): ModuleWithProviders<ToolkitModule> {
    return {
      ngModule: ToolkitModule,
      providers: [
        { provide: 'packageApiUrl', useValue: config.PackageApiUrl },
        { provide: 'packageEventsUrl', useValue: config.PackageEventsUrl }
      ]
    }
  }
}
