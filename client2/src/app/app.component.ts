import { Component, OnInit } from '@angular/core';
import { SignalrService } from './core/services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'client';

  constructor() {}

  ngOnInit(): void {}
}
