import { IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { FieldRequirment } from './FieldRequirment';
import { IconAndValidationField } from './IconAndValidationField';

export class NumberField extends IconAndValidationField<number> {
    readonly isNumber = true;
    public constructor(id: string, name: string, value: number, icon?: IconDefinition, fieldRequirments?: FieldRequirment[]) {
        super(id, name, value, icon, fieldRequirments);
    }
}
