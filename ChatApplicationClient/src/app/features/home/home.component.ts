import { SignalrService } from '../../core/services/signalr.service';
import { User } from '../../shared/models/user.model';
import { Component, NgModule, OnDestroy, OnInit } from '@angular/core';
import { Message } from '../../shared/models/message.model';
import { HttpClient } from '@angular/common/http';
import { CommonModule, DatePipe, NgClass, NgFor } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  standalone: true,
  imports: [DatePipe, NgClass, CommonModule, NgFor]
})
export class HomeComponent implements OnInit, OnDestroy {
  
  onlineUsers: Array<User> = [];

  constructor(private http: HttpClient, private signalRService: SignalrService){
  }

  ngOnInit(): void {

    this.userOnListener();
    this.userOffListener();
    this.getOnlineUsersLis();

    setTimeout(() => {
      this.getOnlineUsersInv();
    }, 1000);
  }

  ngOnDestroy(): void {
    this.logOut();
  }

  userOnListener(): void {
    this.signalRService.hubConnection.on("userOn", (newUser: User) => {
      this.onlineUsers.push(newUser);
    });
  }
  userOffListener(): void {
    this.signalRService.hubConnection.on("userOff", (userId: number) => {
      this.onlineUsers = [...this.onlineUsers.filter(u => u.id != userId)];
    });
  }

  getOnlineUsersInv(): void {
    this.signalRService.hubConnection.invoke("GetOnlineUsers")
    .catch(err => console.log(err));
  }

  getOnlineUsersLis(): void {
    this.signalRService.hubConnection.on("getOnlineUsersResponse", (tempOnlineUsers: Array<User>) => {
      this.onlineUsers = tempOnlineUsers;
      console.log("Got online users");
    });
  }

  async logOut() {
    await this.signalRService.hubConnection.invoke("LogOut")
    .catch(err => console.log(err));
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
