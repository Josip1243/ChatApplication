import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  constructor(private snackBar: MatSnackBar) {}

  showSnackbar(message: string, type: 'error' | 'info' | 'success') {
    this.snackBar.open(message, 'Dismiss', {
      duration: 3000, // Display for 3 seconds
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: [type + '-snackbar'],
    });
  }
}
