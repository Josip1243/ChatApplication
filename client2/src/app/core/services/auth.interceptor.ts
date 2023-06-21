import {
  HTTP_INTERCEPTORS,
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  Observable,
  catchError,
  switchMap,
  throwError,
  EMPTY,
  BehaviorSubject,
  filter,
  take,
} from 'rxjs';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';
import { TokenDTO } from 'src/app/shared/models/tokenDTO.model';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(
    null
  );

  constructor(private authService: AuthService, private router: Router) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const accessToken = this.authService.getAccessToken();

    if (accessToken) {
      request = request.clone({
        setHeaders: { Authorization: `Bearer ${accessToken}` },
      });
    }
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (
          error instanceof HttpErrorResponse &&
          error.status === 401 &&
          !request.url.includes('login')
        ) {
          return this.handle401Error(request, next);
        }
        return throwError(() => new Error(error.error));
      })
    );
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      let tokenDTO = new TokenDTO();
      tokenDTO.accessToken = this.authService.getAccessToken()!;
      tokenDTO.refreshToken = this.authService.getRefreshToken()!;

      if (tokenDTO.refreshToken) {
        return this.authService.refreshToken(tokenDTO).pipe(
          switchMap((token: TokenDTO) => {
            this.authService.storeRefreshToken(token.refreshToken);
            this.authService.storeAccessToken(token.accessToken);
            this.refreshTokenSubject.next(token.accessToken);

            this.isRefreshing = false;

            return next.handle(this.addTokenHeader(request, token.accessToken));
          }),
          catchError((err) => {
            this.isRefreshing = false;
            this.authService.logout('login');
            return throwError(() => new Error(err));
          })
        );
      }
    }

    return this.refreshTokenSubject.pipe(
      filter((token) => token != null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token)))
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string) {
    return request.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }
}

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
];
