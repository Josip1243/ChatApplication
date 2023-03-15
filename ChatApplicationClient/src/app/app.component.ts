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
    this.signalrService.startConnection();

    // setTimeout(() => {
    //   this.signalrService.serverListener();
    // }, 2000);
  }

  // sendMessage() {
  //   this.signalrService.askServer();
  // }
}
