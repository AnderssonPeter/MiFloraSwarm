//Generated file do not make manual changes, they will be lost!

export enum StringSettingType {
    Normal,
    Cron,
    IPAddressOrHostname,
    Password,
}

export class Setting<T> {
    constructor(public readonly name: string, public readonly isRequired: boolean, public readonly defaultValue: T) { }
}

export class NumberSetting extends Setting<number> { 
    constructor(name: string, isRequired: boolean, defaultValue: number) {
        super(name, isRequired, defaultValue);
    }
}

export class BooleanSetting extends Setting<boolean> { 
    constructor(name: string, isRequired: boolean, defaultValue: boolean) {
        super(name, isRequired, defaultValue);
    }
}

export class StringSetting extends Setting<string> {
    constructor(name: string, isRequired: boolean, defaultValue: string, public readonly stringType: StringSettingType = StringSettingType.Normal) {
        super(name, isRequired, defaultValue);
    }
}

export const SettingDefinitions = [
    new StringSetting('UpdateBatteryAndVersionCron', true, '0 0 0 ? * *', StringSettingType.Cron),
    new StringSetting('UpdateValuesCron', true, '0 0 * ? * *', StringSettingType.Cron),
    new StringSetting('MQTTClientId', true, 'MiFloraGateway', StringSettingType.Normal),
    new StringSetting('MQTTServerAddress', true, '', StringSettingType.IPAddressOrHostname),
    new NumberSetting('MQTTPort', true, 1883),
    new StringSetting('MQTTUsername', false, '', StringSettingType.Normal),
    new StringSetting('MQTTPassword', false, '', StringSettingType.Password),
    new BooleanSetting('MQTTUseTLS', true, false),
]
