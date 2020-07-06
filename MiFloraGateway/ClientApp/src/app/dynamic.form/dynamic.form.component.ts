import { Component, OnInit, Input, Output } from '@angular/core';
import DynamicForm from './dynamic.form';
import { FormBuilder, FormGroup } from '@angular/forms';
import { merge } from 'rxjs';
import { first } from 'rxjs/operators';

@Component({
    selector: 'app-dynamic-form',
    templateUrl: './dynamic.form.component.html',
    styleUrls: ['./dynamic.form.component.scss']
})
export class DynamicFormComponent implements OnInit {

    @Input() dynamicForm?: DynamicForm;
    @Input() form?: FormGroup;
    @Input() submitText?: string;
    @Input() showLabels: boolean = true;
    @Input() showPlaceholder: boolean = false;
    message: string = '';
    messageType: 'error' | 'success' = 'error'

    loading = false;
    get isValid() {
        return (this.form?.valid ?? false);
    }

    constructor(private readonly formBuilder: FormBuilder) { }

    ngOnInit(): void {
        this.form = this.dynamicForm?.toFromGroup(this.formBuilder);
        this.submitText = this.submitText ?? 'Save';
    }

    getFields() {
        return this.dynamicForm?.fieldContainer.fields ?? [];
    }

    async onSubmit() {
        this.loading = true;
        try {
            if (!this.dynamicForm) {
                throw new Error('Can\'t save without dynamic form!');
            }
            this.dynamicForm.fieldContainer.fields.forEach(field => field.control.disable());
            const result = await this.dynamicForm.save();
            if ('errorMessage' in result) {
                //We encountered one or more errors!
                this.message = result.errorMessage;
                this.messageType = 'error';

                result.errors.forEach((error) => {
                    const fields = error.fields.map(fieldId => this.dynamicForm!.getField(fieldId));
                    const observables = fields.map(field => field.addTemporaryError(error.description));
                    merge(...observables).pipe(first()).subscribe((newValue) => {
                        fields.forEach(field => field.removeTemporaryError(error.description));
                    })
                });
            }
            else {
                this.message = result.successMessage;
                this.messageType = 'success';
            }
        }
        finally
        {
            this.loading = false;
            this.dynamicForm?.fieldContainer.fields.forEach(field => field.control.enable());
        }
    }
}
