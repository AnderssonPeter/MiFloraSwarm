import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { faUser, faLock } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public loginForm: FormGroup;
  returnUrl: string;
  loading = false;
  submitted = false;
  error: string;
  icons = { username: faUser, password: faLock }

  constructor(formBuilder: FormBuilder, private router: Router, private route: ActivatedRoute, private authenticationService: AuthenticationService) { 
    this.loginForm = formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  async onSubmit() {
    console.log(this.loginForm);
    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }
    const username = this.loginForm.get('username').value;
    const password = this.loginForm.get('password').value;

    this.loading = true;
    try {
      await this.authenticationService.login(username, password);
      this.router.navigate([this.returnUrl])
    }
    catch(ex) {
      this.error = ex.message;
    }
    finally {
      this.loading = false;
    }
  }
}
