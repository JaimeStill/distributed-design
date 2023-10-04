import {
  FormsModule,
  ReactiveFormsModule
} from '@angular/forms';

import {
  RouteComponents,
  Routes
} from './routes';

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ToolkitModule } from '@distributed/toolkit';
import { CdkModule } from './cdk.module';
import { MaterialModule } from './material.module';
import { AppComponent } from './app.component';
import { environment } from '../environments/environment';

@NgModule({
  declarations: [
    AppComponent,
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
      PackageApiUrl: `${environment.api}package/`,
      PackageEventsUrl: `${environment.events}package/`,
      WorkflowsGatewayUrl: environment.workflowsGateway
    }),
    RouterModule.forRoot(Routes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
