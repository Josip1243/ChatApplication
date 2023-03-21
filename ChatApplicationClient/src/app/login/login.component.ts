import { User } from './../shared/models/user.model';
import { SignalrService } from './../signalr.service';
import { Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
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