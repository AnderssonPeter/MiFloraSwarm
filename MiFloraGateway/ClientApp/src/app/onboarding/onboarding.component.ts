import { Component, OnInit } from '@angular/core';
import { SettingDefinitions, StringSetting, StringSettingType, BooleanSetting, NumberSetting } from '../settings';
import { SetupModel, Settings, ErrorResult } from '../api/rest/rest.client';
import OnboardingService from './onboarding.service';
import DynamicForm from '../dynamic.form/dynamic.form';
import { FieldRequirment } from "../dynamic.form/FieldRequirment";
import { FieldType } from "../dynamic.form/FieldType";
import { Router } from '@angular/router';

@Component({
    selector: 'app-onboarding',
    templateUrl: './onboarding.component.html',
    styleUrls: ['./onboarding.component.scss']
})
export class OnboardingComponent implements OnInit {
    public readonly dynamicForm: DynamicForm;

    constructor(private readonly onboardingService: OnboardingService, private readonly router: Router) {
        this.dynamicForm = new DynamicForm(() => this.saveAsync());
        this.dynamicForm.fieldContainer
            .addField(FieldType.Text, 'Username', 'Username', 'Admin', { fieldRequirments: [ 'Required'] })
            .addField(FieldType.Password, 'Password', 'Password', '', { fieldRequirments: [ 'Required', { minimumLength: 6 } ] })
            .addField(FieldType.Password, 'ConfirmPassword', 'Confirm password', '', { fieldRequirments: [ 'Required', { minimumLength: 6 } ] });

        SettingDefinitions.forEach(settingsDefinition => {
            if (settingsDefinition instanceof StringSetting) {
                const fieldType = settingsDefinition.stringType == StringSettingType.Password ? FieldType.Password : FieldType.Text;
                const fieldRequirments: FieldRequirment[] = [];
                if (settingsDefinition.isRequired) {
                    fieldRequirments.push('Required');
                }
                if (settingsDefinition.stringType == StringSettingType.IPAddressOrHostname) {
                    fieldRequirments.push('IPAddressOrHostname');
                }
                if (settingsDefinition.stringType == StringSettingType.Cron) {
                    fieldRequirments.push('Cron');
                }
                this.dynamicForm.fieldContainer.addField(fieldType, 'Settings.' + settingsDefinition.name, settingsDefinition.name, settingsDefinition.defaultValue, { fieldRequirments });
            }
            else if (settingsDefinition instanceof BooleanSetting) {
                this.dynamicForm.fieldContainer.addField(FieldType.Boolean, 'Settings.' + settingsDefinition.name, settingsDefinition.name, settingsDefinition.defaultValue);
            }
            else if (settingsDefinition instanceof NumberSetting) {
                const fieldRequirments: FieldRequirment[] = [];
                if (settingsDefinition.isRequired) {
                    fieldRequirments.push('Required');
                }
                this.dynamicForm.fieldContainer.addField(FieldType.Number, 'Settings.' + settingsDefinition.name, settingsDefinition.name, settingsDefinition.defaultValue, { fieldRequirments });
            }
        });
    }

    async saveAsync() {
        const settings = Object.values<Settings | String>(Settings).filter(x => typeof(x) === 'number') as Settings[];
        const settingsValues = settings.reduce<{ [key in keyof typeof Settings]?: any; }>((previous, current) => {
            const name = Settings[current];
            const id = 'Settings.' + name;
            const value = this.dynamicForm.getValue(id);
            previous[current] = value;
            return previous;
        }, {});

        const username = this.dynamicForm.getStringValue('Username');
        const password = this.dynamicForm.getStringValue('Password');
        const confirmPassword = this.dynamicForm.getStringValue('ConfirmPassword');
        if (confirmPassword != password) {
            return {
                errorMessage: 'Password and Confirm password must be the same',
                errors: [
                    {
                        fields: ['Password', 'ConfirmPassword'],
                        description: 'Password and Confirm password must be the same'
                    }
                ]
            }
        }

        const setupModel: SetupModel = {
            username,
            password,
            settings: settingsValues
        }
        console.log(setupModel);
        try {
            const result = await this.onboardingService.setupAsync(setupModel);
            this.router.navigate(['/login']);
            return result;
        }
        catch(ex) {
            return ex as ErrorResult;
        }
    }

    ngOnInit(): void {
    }
}
