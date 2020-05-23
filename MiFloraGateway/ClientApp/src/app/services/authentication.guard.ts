import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
  })
export class AuthenticationGuard implements CanActivate {
    private isAuthenticated = false;

    constructor(authenticationService: AuthenticationService, private readonly router: Router) {
        authenticationService.currentUser.subscribe(user => this.isAuthenticated = (user != null));
    }

    canActivate(
      next: ActivatedRouteSnapshot,
      state: RouterStateSnapshot): boolean {
        if (!this.isAuthenticated) {
          this.router.navigateByUrl('/login');
        }
        return this.isAuthenticated;
    }
  }