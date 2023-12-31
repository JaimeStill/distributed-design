import { Injectable } from '@angular/core';

import {
  MatSnackBar,
  MatSnackBarConfig
} from '@angular/material/snack-bar';

import { ISnacker } from '@distributed/core';

@Injectable({
  providedIn: 'root'
})
export class SnackerService implements ISnacker {
  private config = new MatSnackBarConfig();

  constructor(private snackbar: MatSnackBar) {
    this.config.duration = 5000;
    this.config.panelClass = [];
  }

  private sendMessage = (message: string) => this.snackbar.open(message, 'Close', this.config);

  setDuration(duration: number) {
    this.config.duration = duration;
  }

  setClasses = (classes: string[]) => {
    classes.push('snacker');
    classes.push('background-card');
    this.config.panelClass = classes;
  }

  sendColorMessage = (message: string, colors: string[]) => {
    this.setClasses(colors);
    this.sendMessage(message);
  }

  sendErrorMessage = (message: string) => {
    this.setClasses(['snacker-red']);
    this.sendMessage(message);
  }

  sendWarningMessage = (message: string) => {
    this.setClasses(['snacker-orange']);
    this.sendMessage(message);
  }

  sendSuccessMessage = (message: string) => {
    this.setClasses(['snacker-green']);
    this.sendMessage(message);
  }
}
