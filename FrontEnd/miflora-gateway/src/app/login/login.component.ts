import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

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

  constructor(formBuilder: FormBuilder, private router: Router, private route: ActivatedRoute) { 
    this.loginForm = formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  onSubmit(): void {
    console.log(this.loginForm);
    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }
    const username = this.loginForm.get('username').value;
    const password = this.loginForm.get('password').value;

    this.loading = true;
  }

}
