import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { User } from 'src/app/shared/models/user.model';
import { JwtHelperService } from '@auth0/angular-jwt';
import { TokenDTO } from 'src/app/shared/models/tokenDTO.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = 'http://localhost:5220/';

  private logged = new BehaviorSubject<boolean>(false);
  isLogged = this.logged.asObservable();
  private usernameBehaviorSubject = new BehaviorSubject<string>('');
  private userIdBehaviorSubject = new BehaviorSubject<number>(-1);
  username = this.usernameBehaviorSubject.asObservable();
  userId = this.userIdBehaviorSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  public register(user: User): Observable<any> {
    return this.http.post(this.baseUrl + 'api/auth/register', user);
  }

  public login(user: User): Observable<any> {
    return this.http.post<any>(this.baseUrl + 'api/auth/login', user).pipe(
      tap(() => {
        this.logged.next(true);
      })
    );
  }

  public logout(page: string) {
    localStorage.clear();
    this.logged.next(false);
    this.router.navigate([page]);
  }

  checkStatus() {
    if (localStorage.getItem('token')) {
      this.logged.next(true);
    } else {
      this.logged.next(false);
    }

    if (this.getAccessToken()) {
      let token = this.decodedToken();
      let username =
        token['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      let userId = Number(token['Id']);
      this.usernameBehaviorSubject.next(username);
      this.userIdBehaviorSubject.next(userId);
    }
  }

  storeAccessToken(tokenValue: string) {
    localStorage.setItem('token', tokenValue);
  }
  storeRefreshToken(tokenValue: string) {
    localStorage.setItem('refreshToken', tokenValue);
  }

  getAccessToken() {
    return localStorage.getItem('token');
  }
  getRefreshToken() {
    return localStorage.getItem('refreshToken');
  }

  decodedToken() {
    const jwtHelper = new JwtHelperService();
    const token = this.getAccessToken()!;
    return jwtHelper.decodeToken(token);
  }

  public refreshToken(tokenDTO: TokenDTO): Observable<TokenDTO> {
    return this.http.post<TokenDTO>(
      this.baseUrl + 'api/auth/refresh-token',
      tokenDTO
    );
  }

  public getMe(): Observable<string> {
    return this.http.get(this.baseUrl + 'api/auth/GetMe', {
      responseType: 'text',
    });
  }
}
