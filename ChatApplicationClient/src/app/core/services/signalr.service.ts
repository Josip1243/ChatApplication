import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { User } from 'src/app/shared/models/user.model';


@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(public router: Router) { }

  hubConnection!: signalR.HubConnection;
  userData!: User;

  name!: String;

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7038/chat', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();
  
      this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch(err => console.log('Error ocurred while connecting: ' + err))
  }
}
