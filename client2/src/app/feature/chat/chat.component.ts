import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ChatService } from 'src/app/core/services/chat.service';
import { ChatNameDTO } from 'src/app/shared/models/chat.models';
import { AuthService } from 'src/app/core/services/auth.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SignalrService } from 'src/app/core/services/signalr.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { SnackbarService } from 'src/app/core/services/snackbar.service';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatListModule,
    FormsModule,
    MatTooltipModule,
  ],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss'],
})
export class UsersComponent implements OnInit {
  chats!: ChatNameDTO[];
  filteredChats!: ChatNameDTO[];
  searchValue: string = '';
  isLoggedIn: boolean = true;
  addChatFlag: boolean = false;
  username!: string;
  @Input() opened!: boolean;
  @Output() openedChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private signalRService: SignalrService,
    private snackbarService: SnackbarService
  ) {}

  ngOnInit(): void {
    this.authService.isLogged.subscribe((logged) => {
      this.isLoggedIn = logged;

      if (logged) {
        this.chatService.allChats.subscribe((c) => {
          this.chats = c;
          this.filteredChats = c;
          this.searchValue = '';
        });
      }
    });

    this.authService.username.subscribe((u) => {
      this.username = u;
    });
  }

  refreshChats() {
    this.chatService.refreshChats();
    this.snackbarService.showSnackbar('Refreshed chats.', 'info');
  }

  changeChat(chatId: number) {
    this.chatService.changeChat(chatId);
    this.openedChange.emit(false);
  }

  addChat(addChatValue: string) {
    if (addChatValue != '') {
      if (addChatValue == this.username) {
        this.snackbarService.showSnackbar('Cannot add yourself.', 'info');
      } else {
        this.signalRService.addChat(this.username, addChatValue);
        this.addChatFlag = false;
      }
    }
  }

  filterChats() {
    this.filteredChats = this.chats.filter((chat) =>
      chat.name.includes(this.searchValue)
    );
  }

  userOnline() {
    return true;
  }

  removeChat(chatId: number) {
    this.chatService.removeChat(chatId).subscribe(() => {
      this.chatService.deleteChat();
      this.snackbarService.showSnackbar('Removed chat.', 'info');
    });
  }
}
