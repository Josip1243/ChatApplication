import { Router } from '@angular/router';
import { SignalrService } from './signalr.service';
import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { HubConnectionState } from '@microsoft/signalr/dist/esm/HubConnection';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  constructor(public signalrService:SignalrService, public router: Router) {}

  ngOnInit(): void {
    this.establishConnection(this.signalrService);
  }

  maxRepetition: number = 0;

  establishConnection(signalrService:SignalrService) {
    this.maxRepetition = this.maxRepetition + 1;
    signalrService.startConnection();

    if(this.maxRepetition < 5){
      setTimeout( () => {

        if (signalrService.hubConnection.state === HubConnectionState.Connected) {
          return;
        }
        this.establishConnection(signalrService);
      }, this.maxRepetition*1000)
    }
  }
}
