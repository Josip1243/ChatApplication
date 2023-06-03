import { SignalrService } from 'src/app/core/services/signalr.service';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnectionState } from '@microsoft/signalr';
import { User } from '../../shared/models/user.model';


@Injectable({ providedIn: 'root' })
export class AuthService {
    constructor(public signalrService: SignalrService, public router: Router) {

          let tempPersonId = localStorage.getItem("personId");
          if (tempPersonId) {
              if (this.signalrService.hubConnection?.state == HubConnectionState.Connected) { //if already connected
                this.reauthMeListener();
                this.reauthMe(tempPersonId);
              }
              // else {
              //   this.signalrService.ssObs().subscribe((obj: any) => {
              //       if (obj.type == "HubConnStarted") {
              //         this.reauthMeListener();
              //         this.reauthMe(tempPersonId);
              //       }
              //   });
              // }
          }
      }


    public isAuthenticated: boolean = false;


    async authMe(person: string, pass: string) {
      let personInfo = {userName: person, password: pass};

      await this.signalrService.hubConnection.invoke("Authorize", personInfo)
      .catch(err => console.error(err));
    }


    authMeListenerSuccess() {
      this.signalrService.hubConnection.on("authMeResponseSuccess", (user: User) => {
        //4Tutorial
        console.log(user);
        this.signalrService.userData = {...user};
        localStorage.setItem("personId", user.id.toString());
        this.isAuthenticated = true;
        this.signalrService.router.navigateByUrl("/home");
      });
    }

    authMeListenerFail() {
      this.signalrService.hubConnection.on("authMeResponseFail", () => {
      });
    }

    async reauthMe(personId: string) {
      await this.signalrService.hubConnection.invoke("reauthMe", personId)
      .catch(err => console.error(err));
    }

    reauthMeListener() {
      this.signalrService.hubConnection.on("reauthMeResponse", (user: User) => {
        //4Tutorial
        console.log(user);
        this.signalrService.userData = {...user}
        this.isAuthenticated = true;
        if (this.signalrService.router.url == "/auth") this.signalrService.router.navigateByUrl("/home");
      });
    }
}