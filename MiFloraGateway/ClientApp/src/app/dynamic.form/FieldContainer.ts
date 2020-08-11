import { IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { FieldType } from './FieldType';
import { FieldRequirment } from './FieldRequirment';
import { BooleanField } from './BooleanField';
import { StringField } from './StringField';
import { NumberField } from './NumberField';

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


    public addField(type: FieldType.Password | FieldType.Text, id: string, name: string, value: string, options?: { icon?: IconDefinition; fieldRequirments?: FieldRequirment[]; }): FieldContainer;
    public addField(type: FieldType.Boolean, id: string, name: string, value: boolean): FieldContainer;
    public addField(type: FieldType.Number, id: string, name: string, value: number, options?: { icon?: IconDefinition; fieldRequirments?: FieldRequirment[]; }): FieldContainer;
    addField(type: FieldType, id: string, name: string, value: string | number | boolean, options?: { icon?: IconDefinition; fieldRequirments?: FieldRequirment[]; }): FieldContainer {
        let field: BooleanField | StringField | NumberField | undefined;
        if (type == FieldType.Boolean) {
            if (typeof (value) != 'boolean') {
                throw new Error('Invalid value expected boolean!');
            }
            field = new BooleanField(id, name, value);
        }
        else if (type == FieldType.Text || type == FieldType.Password) {
            if (typeof (value) != 'string') {
                throw new Error('Invalid value expected string!');
            }
            field = new StringField(id, name, value, type == FieldType.Password, options?.icon, options?.fieldRequirments);
        }
        else if (type == FieldType.Number) {
            if (typeof (value) != 'number') {
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
