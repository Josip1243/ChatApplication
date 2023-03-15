import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';


@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  constructor(public router: Router) { }

  hubConnection!: signalR.HubConnection;

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

  
  // askServer() {
  //   this.hubConnection.invoke("Test", )
  //     .catch(err => console.error(err));
  // }

  // serverListener() {
  //   this.hubConnection.on('serverResponse', (message) => console.log(message));
  // }
}
