import { SignalrService } from './../signalr.service';
import { DataService } from './../services/data.service';
import { User } from './../shared/models/user.model';
import { Component, OnInit } from '@angular/core';
import { Message } from '../shared/models/message.model';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  
  onlineUsers: Array<User> = [];

  constructor(private http: HttpClient, private dataService: DataService, private signalRService: SignalrService){
  }

  ngOnInit(): void {

    this.userOnListener();
    this.userOffListener();
    this.getOnlineUsersLis();


    // this.dataService.getUsers().subscribe(
    //   data => {
    //      this.items.push(data);
    //      console.log("AAAAAAA USPIO");
    //    },
    //    error => {
    //      console.log("ERROOOOOOR");
    //   }
    // );
  }


  userOnListener(): void {
    this.signalRService.hubConnection.on("userOn", (newUser: User) => {
      console.log("new user added");
      this.onlineUsers.push(newUser);
    });
  }

  userOffListener(): void {
    this.signalRService.hubConnection.on("userOff", (userId: number) => {
      this.onlineUsers.filter(u => u.id != userId);
    });
  }

  getOnlineUsersInv(): void {
    this.signalRService.hubConnection.invoke("getOnlineUsers")
    .catch(err => console.log(err));
  }

  getOnlineUsersLis(): void {
    this.signalRService.hubConnection.on("getOnlineUsersResponse", (tempOnlineUsers: Array<User>) => {
      this.onlineUsers = [...this.onlineUsers];
    });
  }







  msgs: Array<Message> = [
    {id: 1, userId: 1, text: 'Ovo je testna poruka', username: 'Josip', timestamp: new Date()},
    {id: 2, userId: 1, text: 'Ovo je jako dugacka testna poruka. Ovo je jako dugacka testna poruka, Ovo je jako dugacka testna poruka.', username: 'Josip', timestamp: new Date()},
    {id: 3, userId: 3, text: 'Ovo je testna poruka', username: 'Marko', timestamp: new Date()}
  ];

  getUsers(): Array<User> {
    return this.onlineUsers;
  }

  getMessages(): Array<Message> {
    return this.msgs;
  }

  isUser(id: number): boolean {
    if(id == 1){
      return true;
    }
    return false;
  }

  isFirstMessage(id: number): boolean {
    if(id == 1){
      return true;
    }
    return false;
  }
}
