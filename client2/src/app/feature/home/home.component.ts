import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from 'src/app/core/services/auth.service';
import { TokenDTO } from 'src/app/shared/models/tokenDTO.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  constructor(private authService: AuthService) {}

  doSomething() {

    let tempAccTok = this.authService.getToken();
    let tempRefTok = this.authService.getRefreshToken();

    let tokentDTO = new TokenDTO();
    tokentDTO.accessToken = tempAccTok == null ? '' : tempAccTok;
    tokentDTO.refreshToken = tempRefTok == null ? '' : tempRefTok;

    this.authService.refreshToken(tokentDTO).subscribe(u => {
      this.authService.storeToken(u.accessToken);
      this.authService.storeRefreshToken(u.refreshToken);
    })
  }

  getMe() {
    this.authService.getMe().subscribe(u => {
      console.log(u);
    })
  }
}
