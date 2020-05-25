import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ReplaySubject, Subject, Observable } from 'rxjs';
import { User } from '../models/user';



@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private readonly currentUserSubject: ReplaySubject<User | undefined>;
  public readonly currentUser: Observable<User | undefined>;

  constructor(private readonly httpClient: HttpClient) {
    this.currentUserSubject = new ReplaySubject<User | undefined>(1);
    this.currentUser = this.currentUserSubject.asObservable();

    httpClient.get<User>('authentication/getCurrentUser')
              .toPromise()
              .then((user) => this.currentUserSubject.next(user))
              .catch((error) => {
                console.log('Failed to get current user!', error);
                this.currentUserSubject.next(undefined);
              });
  }

  async login(username: string, password: string) {
    try {
      const result = await this.httpClient.post<User>('authentication/login', { username, password }, { observe: 'response' })
                            .toPromise();
      if (result.status === 200 && result.body) {
        this.currentUserSubject.next(result.body);
        this.currentUserSubject.subscribe((x) => console.log(x));
      }
      else {
        throw new Error('Invalid username or password!');
      }      
    }
    catch(ex) {
      if (ex instanceof HttpErrorResponse) {
        if (ex.status === 401) {
          throw new Error('Invalid username or password!');
        }
      }
      throw ex;
    }
  }

  async logout() {
    await this.httpClient.post('authentication/logout', null).toPromise();
    this.currentUserSubject.next(undefined);
  }
}
