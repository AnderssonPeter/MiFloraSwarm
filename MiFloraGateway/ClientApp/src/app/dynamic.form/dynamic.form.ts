import { FormBuilder, FormControl, ValidationErrors } from "@angular/forms";
import { ErrorResult, SuccessResult } from '../api/rest/rest.client';
import { StringField } from './StringField';
import { FieldContainer } from './FieldContainer';

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