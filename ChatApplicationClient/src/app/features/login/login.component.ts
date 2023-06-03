import { SignalrService } from '../../core/services/signalr.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {MatIconModule} from '@angular/material/icon';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatIconModule],
})

export class LoginComponent implements OnInit, OnDestroy {
  constructor(public signalRService: SignalrService, public authService: AuthService) {}

  ngOnInit(): void {
    this.authService.authMeListenerSuccess();
    this.authService.authMeListenerFail();
  }

  ngOnDestroy(): void {
    this.signalRService.hubConnection.off("authMeResponseSuccess");
    this.signalRService.hubConnection.off("authMeResponseFail");
  }

  onSubmit(form: NgForm){
    if (!form.valid){
      return;
    }

    this.authService.authMe(form.value.username, form.value.password);
    form.reset();
  }
}