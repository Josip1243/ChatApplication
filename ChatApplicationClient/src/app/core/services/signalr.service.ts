import { EventEmitter, Injectable, OnInit, Output } from '@angular/core';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from '@microsoft/signalr';
import { ChatService } from './chat.service';
import { MessageDTO } from 'src/app/shared/models/messageDTO';
import { SnackbarService } from './snackbar.service';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  hubConnection!: HubConnection;
  @Output() onSignalRMessage: EventEmitter<any> = new EventEmitter();

  constructor(
    private chatService: ChatService,
    private snackbarService: SnackbarService
  ) {}

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
      .catch((err) => console.log('Error while connecting to server: ' + err));

    this.hubConnection.onclose((error) => {
      console.log('SignalR connection closed: ', error);
    });
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
      .invoke('connect', userId)
      .catch(() => console.log('Error while asking server.'));
  }

  askServerListener() {
    this.hubConnection.on('connectListener', (msg) => {
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
      debugger;
      console.log(msg);
      this.stopConnection();
      this.chatService.resetChatService();
    });
  }
  onlineStatusListener() {
    this.hubConnection.on('onlineStatusChange', () => {
      this.chatService.refreshChats();
    });
  }

  testListener() {
    this.hubConnection.on('test', () => {
      debugger;
      console.log('TEEEEEEEEEESSSSSSSSSTTTTTTTT!!!!!!!!!!');
    });
  }

  addChat(currentUsername: string, targetedUsername: string) {
    this.hubConnection
      .invoke('addChat', currentUsername, targetedUsername)
      .catch((err) => {
        console.log('Error while adding chat. ' + err);
        this.snackbarService.showSnackbar('No user found.', 'info');
      });
  }

  addChatListener() {
    this.hubConnection.on('addChatListener', () => {
      this.chatService.refreshChats();
      this.snackbarService.showSnackbar('Chat added.', 'info');
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
