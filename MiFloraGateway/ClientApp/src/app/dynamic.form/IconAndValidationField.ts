import { FormControl, Validators, ValidatorFn } from "@angular/forms";
import { IconDefinition, faClock, faWaveSquare, faUser, faLock, faQuestion } from '@fortawesome/free-solid-svg-icons';
import { IPAddressOrHostnameRegex, IPAddressRegex } from '../regexes';
import { FieldRequirment } from './FieldRequirment';
import { isMinimumLength } from './isMinimumLength';
import { Field } from './Field';

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
