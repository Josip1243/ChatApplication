import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AppRoutingModule } from 'src/app/app-routing.module';
import {MatToolbarModule} from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from 'src/app/core/services/auth.service';


@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule, 
    MatSidenavModule, 
    MatToolbarModule, 
    MatIconModule, 
    MatButtonModule,
    MatMenuModule,
    AppRoutingModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit{
  opened!: boolean;
  isLoggedIn: boolean = true;
  username: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.isLogged.subscribe(logged => {
      this.isLoggedIn = logged;
    });
    this.authService.username.subscribe(name => {
      this.username = name;
    });
    this.authService.checkStatus();
  }

  logout() {
    this.authService.logout('login');
  }
}
