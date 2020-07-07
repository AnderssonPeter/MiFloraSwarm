import { IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { FieldRequirment } from './FieldRequirment';
import { IconAndValidationField } from './IconAndValidationField';

export class StringField extends IconAndValidationField<string> {
    readonly isString = true;


    public constructor(id: string, name: string, value: string, readonly isPassword: boolean, icon?: IconDefinition, fieldRequirments?: FieldRequirment[]) {
        super(id, name, value, icon, fieldRequirments);
    }
}
