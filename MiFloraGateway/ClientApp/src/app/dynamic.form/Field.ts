import { FormControl, AbstractControl } from "@angular/forms";
import { first, distinct, skip } from 'rxjs/operators';

export abstract class Field<T> {
    private innerControl?: FormControl;
    private initialValue: T;
    private readonly temporaryErrors: string[] = [];


    public get control(): FormControl {
        if (!this.innerControl) {
            this.innerControl = this.createControl();
        }
        return this.innerControl;
    }


    public get value(): T {
        return this.innerControl?.value as unknown as T ?? this.initialValue;
    }


    public get errorMessage(): string {
        //todo: Handle setTemporaryErrors!
        return this.getErrors().join(', ');
    }


    public constructor(readonly id: string, readonly name: string, value: T) {
        this.initialValue = value;
    }


    protected createControl(): FormControl {
        return new FormControl(this.initialValue, (control) => this.temporaryErrorsValidationFunction(control));
    }


    protected temporaryErrorsValidationFunction(control: AbstractControl) {
        if (this.temporaryErrors.length > 0) {
            return { 'temporary': this.temporaryErrors };
        }
        return null;
    }


    protected getErrors() {
        const errors = this.control.errors;
        if (errors && errors.temporary) {
            return errors.temporary as string[];
        }
        return [''];
    }


    public addTemporaryError(error: string) {
        this.temporaryErrors.push(error);
        this.control.markAsTouched();
        this.control.updateValueAndValidity();
        return this.control.valueChanges.pipe(distinct()).pipe(skip(1)).pipe(first());
    }


    public removeTemporaryError(error: string) {
        let index = this.temporaryErrors.indexOf(error);
        while (index != -1) {
            this.temporaryErrors.splice(index);
            index = this.temporaryErrors.indexOf(error);
        }
        this.control.updateValueAndValidity();
    }
}
