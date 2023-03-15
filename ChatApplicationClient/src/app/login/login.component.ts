import { User } from './../shared/models/user.model';
import { SignalrService } from './../signalr.service';
import { Router } from '@angular/router';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit, OnDestroy {
  constructor(public router: Router, private signalRService: SignalrService) {}

  ngOnInit(): void {
    this.authMeListenerSucces();
    this.authMeListenerFail();
  }

  ngOnDestroy(): void {
    this.signalRService.hubConnection.off("authMeResponseSuccess");
    this.signalRService.hubConnection.off("authMeResponseFail");
  }

  onSubmit(form: NgForm){
    if (!form.valid){
      return;
    }

    this.authMe(form.value.username, form.value.password);
    form.reset();
  }

  private authMeListenerSucces() {
    this.signalRService.hubConnection.on("authMeResponseSuccess", (userInfo: any) => {
      console.log(userInfo);

      this.signalRService.name = userInfo.name;
      console.log("Successfully logged in!");
      this.router.navigateByUrl("/home");
    })
  }

  private authMeListenerFail() {
    this.signalRService.hubConnection.on("authMeResponseFail", () => {
      console.error("Wrong credentials!");
    })
  }

  async authMe(username: String, password: String){
    let userInfo = {username: username, password: password}

    console.log("Loging in atempt...");
    await this.signalRService.hubConnection.invoke("Authorize", userInfo)
    .catch(err => console.log(err));
  }
}