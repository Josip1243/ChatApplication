import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Route, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  loggedIn: boolean = false;

  constructor(private authService: AuthService, private router: Router) {
    this.authService.isLogged.subscribe(logged => {
      this.loggedIn = logged;
    })
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

      this.authService.checkStatus();

      debugger;

      if (this.loggedIn) {
        return true;
      }
      this.router.navigate(['login']);
      return false;
  }
}
