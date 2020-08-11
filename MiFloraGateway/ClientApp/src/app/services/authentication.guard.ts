import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationService } from './authentication.service';
import { Injectable } from '@angular/core';
import { first } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
  })
export class AuthenticationGuard implements CanActivate {
    constructor(private readonly authenticationService: AuthenticationService, private readonly router: Router) {
    }

    async canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
      const user = await this.authenticationService.currentUser.pipe(first()).toPromise();
      const requiresAdmin = (next.data.requiresAdmin ?? false) as boolean;
      if (user == undefined || (requiresAdmin && !user.isAdmin)) {
        this.router.navigateByUrl('/login');
        return false;
      }
      return true;
    }
}
