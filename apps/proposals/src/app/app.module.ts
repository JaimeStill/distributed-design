import {
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ToolkitModule } from '@distributed/toolkit';

import {
  RouteComponents,
  Routes
} from './routes';

import { CdkModule } from './cdk.module';
import { MaterialModule } from './material.module';
import { AppComponent } from './app.component';
import { environment } from '../environments/environment';
import { Components } from './components';
import { Dialogs } from './dialogs';
import { Forms } from './forms';

@NgModule({
  declarations: [
    AppComponent,
    ...Components,
    ...Dialogs,
    ...Forms,
    ...RouteComponents
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    CdkModule,
    MaterialModule,
    ToolkitModule.forRoot({
      PackageApiUrl: environment.packageApi,
      PackageEventsUrl: environment.packageEvents,
      WorkflowsGatewayUrl: environment.workflowsGateway
    }),
    RouterModule.forRoot(Routes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
