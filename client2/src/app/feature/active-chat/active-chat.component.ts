import { CommonModule, NgFor } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from 'src/app/core/services/auth.service';
import { ChatService } from 'src/app/core/services/chat.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { ChatDTO, Message } from 'src/app/shared/models/chat.models';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  templateUrl: './active-chat.component.html',
  styleUrls: ['./active-chat.component.scss'],
})
export class ChatComponent implements OnInit {
  chatRoom!: ChatDTO;
  username!: string;

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private signalrService: SignalrService
  ) {}

  ngOnInit(): void {
    this.chatService.currentChat.subscribe((num) => {
      this.chatService.getChat(num).subscribe((chat) => {
        this.chatRoom = chat;
      });
    });

    this.authService.username.subscribe((u) => {
      this.username = u;
    });
  }

  showMessageSender(message: Message, i: number): boolean {
    if (i == 0 && this.chatRoom.messages[i].username != message.username) {
      return true;
    } else if (this.chatRoom.messages[i].username == message.username) {
      return false;
    }
    return true;
  }

  sendMessage() {
    this.signalrService.sendMessage('Hello', this.chatRoom.id);
  }
}
