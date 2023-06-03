import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, catchError, switchMap, throwError } from "rxjs";
import { AuthService } from "./auth.service";
import { Router } from "@angular/router";
import { TokenDTO } from "src/app/shared/models/tokenDTO.model";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private authService: AuthService, private router: Router) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const token = this.authService.getToken();

        if (token) {
            req = req.clone({
                setHeaders: { Authorization: `Bearer ${token}` }
            });
        }
        return next.handle(req).pipe(
          catchError((err:any) => {
            if (err instanceof HttpErrorResponse){
              if (err.status === 401) {
                debugger;

                let tokenDTO = new TokenDTO();
                tokenDTO.accessToken = this.authService.getToken()!;
                tokenDTO.refreshToken = this.authService.getRefreshToken()!;

                return this.authService.refreshToken(tokenDTO)
                  .pipe(
                    switchMap((data:TokenDTO)=>{
                      this.authService.storeRefreshToken(data.refreshToken);
                      this.authService.storeToken(data.accessToken);
                      req = req.clone({
                        setHeaders: {Authorization:`Bearer ${data.accessToken}`}  // "Bearer "+myToken
                      })
                      return next.handle(req);
                    }),
                    catchError((err)=>{
                      return throwError(()=>{
                        console.log("Token is expired, Please Login again");
                        this.authService.logout('login');
                      })
                    })
                  )
              }
              console.log(err);
              return throwError(() => new Error(err.error))
            }
            return throwError(() => new Error("Unexpected error occured."))
          })
        );
    }
}