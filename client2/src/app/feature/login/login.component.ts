import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from 'src/app/core/services/auth.service';
import { User } from 'src/app/shared/models/user.model';
import { Router } from '@angular/router';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { SnackbarService } from 'src/app/core/services/snackbar.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  hidePassword = true;
  loginForm!: FormGroup;
  user = new User();
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router,
    private signalrService: SignalrService,
    private snackbarService: SnackbarService
  ) {}

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
    });
    this.authService.logout('login');
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.authService.login(this.user).subscribe({
        next: (tokenDTO) => {
          this.loginForm.reset();
          this.authService.storeAccessToken(tokenDTO.accessToken);
          this.authService.storeRefreshToken(tokenDTO.refreshToken);
          this.signalrService.startConnection();
          setTimeout(() => {}, 1000);
          this.router.navigateByUrl('home');
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
