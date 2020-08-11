import { Field } from './Field';

export class BooleanField extends Field<Boolean> {
    readonly isBoolean = true;
    public constructor(id: string, name: string, value: Boolean) {
        super(id, name, value);
    }
}
