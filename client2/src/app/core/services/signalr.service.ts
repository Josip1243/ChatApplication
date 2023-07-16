import { EventEmitter, Injectable, OnInit, Output } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { ChatService } from './chat.service';
import { MessageDTO } from 'src/app/shared/models/messageDTO';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  userId!: number;
  hubConnection!: HubConnection;
  @Output() onSignalRMessage: EventEmitter<any> = new EventEmitter();

  constructor(
    private authService: AuthService,
    private chatService: ChatService
  ) {
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

  sendMessage(message: MessageDTO) {
    this.hubConnection
      .invoke('sendMessage', message)
      .catch(() => console.log('Error while sending message.'));
  }

  receiveMessage() {
    this.hubConnection.on('receiveMessage', (msg) => {
      this.onSignalRMessage.emit(msg);
    });
  }

  refreshChatList() {
    this.hubConnection.on('refreshChatList', () => {
      this.chatService.getAllChats();
    });
  }
}
