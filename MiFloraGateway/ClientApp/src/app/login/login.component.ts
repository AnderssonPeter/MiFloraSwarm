import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { faUser, faLock } from '@fortawesome/free-solid-svg-icons';
import { first, filter } from 'rxjs/operators';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public form: FormGroup;
  returnUrl?: string;
  loading = false;
  error?: string;
  icons = {
    username: faUser,
    password: faLock
  };

  constructor(formBuilder: FormBuilder, private readonly router: Router, private readonly route: ActivatedRoute, private readonly authenticationService: AuthenticationService) {
    this.form = formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.authenticationService.currentUser.pipe(first(), filter(user => !!user))
                                          .subscribe(() => this.router.navigate([this.returnUrl]));
  }

  async onSubmit() {
    console.log(this.form);
    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }
    const username = this.form.get('username')?.value;
    const password = this.form.get('password')?.value;

    this.loading = true;
    try {
      await this.authenticationService.login(username, password);
      this.router.navigate([this.returnUrl]);
    }
    catch(ex) {
      this.error = ex.message;
    }
    finally {
      this.loading = false;
    }
  }
}
