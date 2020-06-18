import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ReplaySubject, Observable } from 'rxjs';
import { AuthenticationClient, UserModel, ApiException } from '../api/rest/RestClient';



@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private readonly currentUserSubject = new ReplaySubject<UserModel | undefined>(1);
  public readonly currentUser = this.currentUserSubject.asObservable();

  constructor(private readonly authenticationClient: AuthenticationClient) {
    this.currentUserSubject = new ReplaySubject<UserModel | undefined>(1);
    this.currentUser = this.currentUserSubject.asObservable();

    authenticationClient.getCurrentUser()
                        .toPromise()
                        .then(user => this.currentUserSubject.next(user))
                        .catch((error) => {
                            console.log('Failed to get current user!', error);
                            this.currentUserSubject.next(undefined);
                        });
  }

  async login(username: string, password: string) {
    try {
      const result = await this.authenticationClient.login(username, password).toPromise();
      this.currentUserSubject.next(result);
    }
    catch(ex) {
      if (ex instanceof HttpErrorResponse || ex instanceof ApiException) {
        if (ex.status === 401) {
          throw new Error('Invalid username or password!');
        }
      }
      throw ex;
    }
  }

  async logout() {
    await this.authenticationClient.logout().toPromise();
    this.currentUserSubject.next(undefined);
  }
}
