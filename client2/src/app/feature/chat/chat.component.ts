import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ChatService } from 'src/app/core/services/chat.service';
import { ChatNameDTO } from 'src/app/shared/models/chat.models';
import { AuthService } from 'src/app/core/services/auth.service';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';


@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, MatFormFieldModule, MatIconModule, MatInputModule, MatButtonModule, ReactiveFormsModule, MatListModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class UsersComponent implements OnInit {

  chats!: ChatNameDTO[];
  isLoggedIn: boolean = true;
  addChatFlag: boolean = false;

  constructor(private chatService: ChatService, private authService: AuthService) {}

  ngOnInit(): void {

    this.authService.isLogged.subscribe(logged => {
      this.isLoggedIn = logged;

      if(logged) {
        this.chatService.getAllChats().subscribe(c => {
          this.chats = c;
        })
      }
    });
  }

  refreshChats() {
    this.chatService.getAllChats().subscribe(c => {
      this.chats = c;
    })
  }

  changeChat(chatId: number) {
    this.chatService.changeChat(chatId);
  }

  addChat(searchValue: string) {
    this.chatService.addChat(searchValue);
  }

  removeChat(chatId: number) {
    this.chatService.removeChat(chatId);
  }
}
