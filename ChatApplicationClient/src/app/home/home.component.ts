import { User } from './../shared/models/user.model';
import { Component } from '@angular/core';
import { Message } from '../shared/models/message.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  items: Array<User> = [
    {id: 1, username: 'Josip'},
    {id: 2, username: 'Marko'},
    {id: 3, username: 'Pero'}
  ];

  msgs: Array<Message> = [
    {id: 1, userId: 1, text: 'bok'},
    {id: 2, userId: 2, text: 'bok'},
    {id: 3, userId: 3, text: 'bok'}
  ];

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
