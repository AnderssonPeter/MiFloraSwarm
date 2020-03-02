import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, delay } from 'rxjs/operators';
import { from, throwError } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;
  
  constructor() {
    this.currentUserSubject = new BehaviorSubject<User>(null);
    this.currentUser = this.currentUserSubject.asObservable();
   }


  login(username: string, password: string) {
    if (username.toLowerCase() == 'admin' && password.toLowerCase() == 'qwerty') {
      return from([new User(username, password)])
        .pipe(delay(2000))
        .pipe(map(user => {
          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);
          return user;
        }));
    }
    else {
      return throwError(new Error('Invalid username or password!'))
        .pipe(delay(2000));
    }

    /*return this.http.post<any>(`${config.apiUrl}/users/authenticate`, { username, password })
        .pipe(map(user => {
            // store user details and jwt token in local storage to keep user logged in between page refreshes
            localStorage.setItem('currentUser', JSON.stringify(user));
            this.currentUserSubject.next(user);
            return user;
        }));*/
  }

  logout() {
      // remove user from local storage and set current user to null
      localStorage.removeItem('currentUser');
      this.currentUserSubject.next(null);
  }
}
