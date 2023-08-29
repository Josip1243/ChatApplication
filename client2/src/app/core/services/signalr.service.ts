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
  hubConnection!: HubConnection;
  @Output() onSignalRMessage: EventEmitter<any> = new EventEmitter();

  constructor(private chatService: ChatService) {}

  startConnection = () => {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5220/chat', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
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

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection
        .stop()
        .catch(() => console.log('Error while stopping connection.'));
    }
  }

  askServer(userId: number) {
    this.hubConnection
      .invoke('askServer', userId)
      .catch(() => console.log('Error while asking server.'));
  }

  askServerListener() {
    this.hubConnection.on('askServerListener', (msg) => {
      console.log(msg);
    });
  }

  disconnect() {
    this.hubConnection
      .invoke('disconnect')
      .catch((err) =>
        console.log('Error while disconnecting from server.' + err)
      );
  }
  disconnectListener() {
    this.hubConnection.on('disconnect', (msg) => {
      console.log(msg);
      this.stopConnection();
      this.chatService.resetChatService();
    });
  }

  addChat(currentUsername: string, targetedUsername: string) {
    this.hubConnection
      .invoke('addChat', currentUsername, targetedUsername)
      .catch((err) => console.log('Error while adding chat.' + err));
  }

  addChatListener() {
    this.hubConnection.on('addChatListener', () => {
      console.log('Trigered addChatListener');
      this.chatService.refreshChats();
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
      this.chatService.refreshChats();
    });
  }
}
