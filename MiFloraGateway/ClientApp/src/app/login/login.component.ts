import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { faUser, faLock } from '@fortawesome/free-solid-svg-icons';
import { first, filter } from 'rxjs/operators';
import DynamicForm, { FieldType } from '../dynamic.form/dynamic.form';
import { ErrorResult } from '../api/rest/rest.client';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
    public readonly dynamicForm: DynamicForm;

    returnUrl?: string;

    constructor(formBuilder: FormBuilder, private readonly router: Router, private readonly route: ActivatedRoute, private readonly authenticationService: AuthenticationService) {
        this.dynamicForm = new DynamicForm(() => this.loginAsync());
        this.dynamicForm.fieldContainer
            .addField(FieldType.Text, 'Username', 'Username', 'Admin', { fieldRequirments: [ 'Required'] })
            .addField(FieldType.Password, 'Password', 'Password', '', { fieldRequirments: [ 'Required', { minimumLength: 6 } ] });
    }

    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.authenticationService.currentUser.pipe(first(), filter(user => !!user))
                                            .subscribe(() => this.router.navigate([this.returnUrl]));
    }

    async loginAsync() {
        const username = this.dynamicForm.getStringValue('Username');
        const password = this.dynamicForm.getStringValue('Password');
        try {
            await this.authenticationService.login(username, password);
            this.router.navigate([this.returnUrl]);
            return {
                successMessage: 'Logged in'
            }
        }
        catch(ex) {
            return ex as ErrorResult;
        }
        finally {
        }
    }
}
