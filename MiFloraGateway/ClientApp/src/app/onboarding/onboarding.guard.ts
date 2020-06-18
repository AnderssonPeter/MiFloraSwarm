import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { first } from 'rxjs/operators';
import OnboardingService from './OnboardingService';

@Injectable({
    providedIn: 'root'
})
export class OnboardingGuard implements CanActivate {
    constructor(private readonly onboardingService: OnboardingService, private readonly router: Router) {
    }

    async canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
      const isSetup = await this.onboardingService.isSetup.pipe(first()).toPromise();
      if (!isSetup) {
        this.router.navigateByUrl('/onboarding');
        return false;
      }
      return true;
    }
}

