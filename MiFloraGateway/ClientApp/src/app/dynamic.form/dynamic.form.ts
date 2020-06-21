import { FormBuilder, FormControl, Validators, AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { IconDefinition, faClock, faWaveSquare, faUser, faLock, faQuestion } from '@fortawesome/free-solid-svg-icons';
import { IPAddressOrHostnameRegex, IPAddressRegex } from '../regexes';
import { ErrorResult, SuccessResult } from '../api/rest/rest.client';
import { first, distinct, skip } from 'rxjs/operators';

export enum FieldType {
    Text,
    Password,
    Number,
    Boolean
}

export type FieldRequirment = 'Required' | 'Cron' | 'IPAddressOrHostname' | { minimumLength: number };

function isMinimumLength(x: any): x is { minimumLength: number } {
    return typeof(x) === 'object' && x != null && 'minimumLength' in x && typeof(x.minimumLength) === 'number';
}

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
            return { 'temporary': this.temporaryErrors }
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
        while(index != -1) {
            this.temporaryErrors.splice(index);
            index = this.temporaryErrors.indexOf(error);
        }
        this.control.updateValueAndValidity();
    }
}

export class BooleanField extends Field<Boolean> {
    readonly isBoolean = true;
    public constructor(id: string, name: string, value: Boolean) {
        super(id, name, value);
    }
}

export class IconAndValidationField<T> extends Field<T> {
    readonly icon: IconDefinition;
    readonly fieldRequirments: FieldRequirment[];

    public constructor(id: string, name: string, value: T, icon?: IconDefinition, fieldRequirments?: FieldRequirment[]) {
        super(id, name, value);
        this.icon = icon ?? this.getFallbackIcon(name);
        this.fieldRequirments = fieldRequirments ?? [];
    }

    private getValidators() {
        const validators: ValidatorFn[] = [(control) => this.temporaryErrorsValidationFunction(control)];
        if (this.fieldRequirments.includes('Required')) {
            validators.push(Validators.required);
        }
        if (this.fieldRequirments.includes('Cron')) {
            //result.push(Validators.cron?);
        }
        if (this.fieldRequirments.includes('IPAddressOrHostname')) {
            validators.push(Validators.pattern(IPAddressOrHostnameRegex));
        }
        const minLength = this.fieldRequirments.find(isMinimumLength);
        if (minLength) {
            validators.push(Validators.minLength(minLength.minimumLength));
        }
        return validators;
    }

    private getFallbackIcon(name: string) {
        if (name.toLowerCase().includes("cron"))
            return faClock;
        else if (name.toLowerCase().includes("mqtt"))
            return faWaveSquare;
        if (name.toLowerCase() === "username")
            return faUser;
        if (name.toLowerCase().includes("password"))
            return faLock;
        return faQuestion;
    }

    protected createControl() {
        return new FormControl(this.value, this.getValidators());
    }

    protected getErrors() {
        const errors = this.control?.errors;
        if (!errors || !this.control?.touched) {
            return [];
        }

        if (errors.required) {
            return [`${this.name} is required`];
        }

        const result = super.getErrors();
        if (errors.pattern) {
            if (errors.pattern.requiredPattern == IPAddressRegex) {
                result.push(`${this.name} is not a valid ip address`);
            }
            else if (errors.pattern.requiredPattern == IPAddressOrHostnameRegex) {
                result.push(`${this.name} is not a valid ip address or hostname`);
            }
        }
        else if (errors.minlength) {
            result.push(`${this.name} must be atleast ${errors.minlength.requiredLength} characters long`);
        }
        return result;
    }
}

export class StringField extends IconAndValidationField<string> {
    readonly isString = true;

    public constructor(id: string, name: string, value: string, readonly isPassword: boolean, icon?: IconDefinition, fieldRequirments?: FieldRequirment[]) {
        super(id, name, value, icon, fieldRequirments);
    }
}

export class NumberField extends IconAndValidationField<number> {
    readonly isNumber = true;
    public constructor(id: string, name: string, value: number, icon?: IconDefinition, fieldRequirments?: FieldRequirment[]) {
        super(id, name, value, icon, fieldRequirments);
    }
}

export class FieldContainer {
    private readonly innerFields: (BooleanField | NumberField | StringField)[] = [];
    public readonly fields: ReadonlyArray<BooleanField | NumberField | StringField> = this.innerFields;

    public getById(id: string) {
        const field = this.fields.find(x => x.id == id);
        if (field == undefined) {
            throw new Error("Failed to find field with id " + id);
        }
        return field;
    }

    public addField(type: FieldType.Password | FieldType.Text, id: string, name: string, value: string, options?: { icon?: IconDefinition; fieldRequirments?: FieldRequirment[] }): FieldContainer;
    public addField(type: FieldType.Boolean, id: string, name: string, value: boolean): FieldContainer;
    public addField(type: FieldType.Number, id: string, name: string, value: number, options?: { icon?: IconDefinition; fieldRequirments?: FieldRequirment[] }): FieldContainer;
    addField(type: FieldType, id: string, name: string, value: string | number | boolean, options?: { icon?: IconDefinition; fieldRequirments?: FieldRequirment[] }) : FieldContainer {
        let field: BooleanField | StringField | NumberField | undefined;
        if (type == FieldType.Boolean) {
            if (typeof(value) != 'boolean') {
                throw new Error('Invalid value expected boolean!');
            }
            field = new BooleanField(id, name, value);
        }
        else if (type == FieldType.Text || type == FieldType.Password) {
            if (typeof(value) != 'string') {
                throw new Error('Invalid value expected string!');
            }
            field = new StringField(id, name, value, type == FieldType.Password, options?.icon, options?.fieldRequirments);
        }
        else if (type == FieldType.Number) {
            if (typeof(value) != 'number') {
                throw new Error('Invalid value expected number!');
            }
            field = new NumberField(id, name, value, options?.icon, options?.fieldRequirments);
        }
        if (field) {
            this.innerFields.push(field);
            return this;
        }
        throw new Error('Type is not supported!');
    }
}

export default class DynamicForm {
    public readonly fieldContainer = new FieldContainer();

    public constructor(public readonly save: () => Promise<ErrorResult | SuccessResult>) {

    }

    public getValue(id: string) {
        const field = this.getField(id);
        return field.value;
    }

    public getStringValue(id: string) {
        const field = this.getField(id);
        if (field instanceof StringField)
            return field.value;
        throw new Error('Invalid field type');
    }

    public getBooleanValue(id: string) {
        const field = this.getField(id);
        if (field instanceof StringField)
            return field.value;
        throw new Error('Invalid field type');
    }

    public getField(id: string) {
        const field = this.fieldContainer.getById(id);
        if (!field) {
            throw new Error('Field not found!')
        }
        return field;
    }

    public toFromGroup(formBuilder: FormBuilder) {
        const formData = this.fieldContainer.fields.reduce<{[key: string]: FormControl}>((previous, current) => {
            previous[current.id] = current.control;
            return previous;
        },{});

        return formBuilder.group(formData);
    }
}