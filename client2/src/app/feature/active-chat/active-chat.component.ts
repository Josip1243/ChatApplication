import { CommonModule, NgFor } from '@angular/common';
import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from 'src/app/core/services/auth.service';
import { ChatService } from 'src/app/core/services/chat.service';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { ChatDTO, Message } from 'src/app/shared/models/chat.models';
import { MessageDTO } from 'src/app/shared/models/messageDTO';

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
  text!: string;
  shouldStickToBottom = true;
  @ViewChild('chatBody') chatBody!: ElementRef;

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

    this.signalrService.onSignalRMessage.subscribe((msg) => {
      if (msg.chatId == this.chatRoom.id) {
        this.chatRoom.messages.push(msg);
        this.scrollToBottom();
      }
    });
  }
  ngAfterViewInit() {
    this.scrollToBottom();
  }
  onScroll() {
    debugger;

    const chatBodyElement = this.chatBody.nativeElement;
    const scrollPosition = chatBodyElement.scrollTop;
    const scrollHeight = chatBodyElement.scrollHeight;
    const clientHeight = chatBodyElement.clientHeight;

    this.shouldStickToBottom = scrollPosition >= scrollHeight - clientHeight;
  }
  scrollToBottom() {
    debugger;

    if (this.shouldStickToBottom) {
      setTimeout(() => {
        const chatBodyElement = this.chatBody.nativeElement;
        chatBodyElement.scrollTop = chatBodyElement.scrollHeight;
      }, 900);
    }
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
    let newMessage = new MessageDTO();
    newMessage.chatId = this.chatRoom.id;
    newMessage.content = 'Helllllloooooooo';
    newMessage.sentAt = new Date();
    newMessage.username = this.username;

    this.signalrService.sendMessage(newMessage);
  }
}
