import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from 'src/app/core/services/auth.service';
import { TokenDTO } from 'src/app/shared/models/tokenDTO.model';
import { ChatComponent } from '../active-chat/active-chat.component';
import { UsersComponent } from '../chat/chat.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ChatComponent, UsersComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
  }

  doSomething() {

    let tempAccTok = this.authService.getAccessToken();
    let tempRefTok = this.authService.getRefreshToken();

    let tokentDTO = new TokenDTO();
    tokentDTO.accessToken = tempAccTok == null ? '' : tempAccTok;
    tokentDTO.refreshToken = tempRefTok == null ? '' : tempRefTok;

    this.authService.refreshToken(tokentDTO).subscribe(u => {
      this.authService.storeAccessToken(u.accessToken);
      this.authService.storeRefreshToken(u.refreshToken);
    })
  }

  getMe() {
    this.authService.getMe().subscribe(u => {
      console.log(u);
    })
  }
}
