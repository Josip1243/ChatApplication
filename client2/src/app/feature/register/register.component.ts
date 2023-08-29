import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import {
  FormGroup,
  ReactiveFormsModule,
  FormControl,
  Validators,
} from '@angular/forms';
import { passwordMatch } from 'src/app/core/validators/password-match.validator';
import { AuthService } from 'src/app/core/services/auth.service';
import { User } from 'src/app/shared/models/user.model';
import { Router } from '@angular/router';
import { NgToastModule, NgToastService } from 'ng-angular-popup';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  hidePassword: boolean = true;
  hideConfirmPassword: boolean = true;
  registerForm!: FormGroup;
  user = new User();
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router,
    private snackbarService: SnackbarService
  ) {}

  ngOnInit(): void {
    this.registerForm = new FormGroup(
      {
        username: new FormControl('', [Validators.required]),
        password: new FormControl('', [Validators.required]),
        confirmPassword: new FormControl('', [Validators.required]),
      },
      {
        validators: passwordMatch('password', 'confirmPassword'),
      }
    );
    this.authService.logout('register');
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.authService.register(this.user).subscribe({
        next: () => {
          this.registerForm.reset();

          this.snackbarService.showSnackbar(
            'Successfully registered.',
            'success'
          );

          this.router.navigateByUrl('login');
        },
        error: (e) => {
          console.log(e);
          this.errorMessage = e;
        },
      });
    }
  }

  onCopy(event: ClipboardEvent) {
    event.preventDefault();
    this.snackbarService.showSnackbar(
      'Copying from this input is not allowed.',
      'error'
    );
  }
}
