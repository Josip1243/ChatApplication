import { CommonModule, NgFor } from '@angular/common';
import { Component } from '@angular/core';
import { Message } from 'src/app/shared/models/chat.models';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent {

  msgs: Array<Message> = [
    {id: 1, userId: 1, text: 'Ovo je testna poruka', username: 'Josip', timestamp: new Date()},
    {id: 2, userId: 1, text: 'Ovo je jako dugacka testna poruka. Ovo je jako dugacka testna poruka, Ovo je jako dugacka testna poruka.', username: 'Josip', timestamp: new Date()},
    {id: 3, userId: 3, text: 'Ovo je testna poruka', username: 'Marko', timestamp: new Date()},
    {id: 1, userId: 1, text: 'Ovo je testna poruka', username: 'Josip', timestamp: new Date()},
    {id: 2, userId: 1, text: 'Ovo je jako dugacka testna poruka. Ovo je jako dugacka testna poruka, Ovo je jako dugacka testna poruka.', username: 'Josip', timestamp: new Date()},
    {id: 3, userId: 3, text: 'Ovo je testna poruka', username: 'Marko', timestamp: new Date()},
  ];
}
