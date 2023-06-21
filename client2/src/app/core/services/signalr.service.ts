import { Injectable, OnInit } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from '@microsoft/signalr';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  userId!: number;
  hubConnection!: HubConnection;

  constructor(private authService: AuthService) {
    this.authService.userId.subscribe((id) => {
      this.userId = id;
    });
  }

  startConnection = () => {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5220/chat', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connection started.');
      })
      .catch((err) =>
        console.log('Error while establishing connection: ' + err)
      );
  };

  askServer() {
    this.hubConnection
      .invoke('askServer', this.userId)
      .catch(() => console.log('Error while asking server.'));
  }

  askServerListener() {
    this.hubConnection.on('askServerListener', (msg) => {
      console.log(msg);
    });
  }
}
