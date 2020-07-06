import gql from "graphql-tag";
import { Injectable } from "@angular/core";
import * as Apollo from "apollo-angular";
export type Maybe<T> = T | null;
export type Exact<T extends { [key: string]: any }> = { [K in keyof T]: T[K] };
// Generated on 2020-07-06T22:40:27+02:00

/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
    ID: string;
    String: string;
    Boolean: boolean;
    Int: number;
    Float: number;
    Date: any;
    DateTimeOffset: any;
    TimeSpan: any;
};

export type AddDeviceParameters = {
    readonly __typename?: "AddDeviceParameters";
    readonly mACAddress?: Maybe<Scalars["String"]>;
    readonly iPAddress?: Maybe<Scalars["String"]>;
    readonly port: Scalars["Int"];
    readonly name?: Maybe<Scalars["String"]>;
};

export type AddPlantParameters = {
    readonly __typename?: "AddPlantParameters";
    readonly latinName?: Maybe<Scalars["String"]>;
    readonly alias?: Maybe<Scalars["String"]>;
    readonly display?: Maybe<Scalars["String"]>;
    readonly imageUrl?: Maybe<Scalars["String"]>;
    readonly blooming?: Maybe<Scalars["String"]>;
    readonly category?: Maybe<Scalars["String"]>;
    readonly color?: Maybe<Scalars["String"]>;
    readonly floralLanguage?: Maybe<Scalars["String"]>;
    readonly origin?: Maybe<Scalars["String"]>;
    readonly production?: Maybe<Scalars["String"]>;
    readonly fertilization?: Maybe<Scalars["String"]>;
    readonly pruning?: Maybe<Scalars["String"]>;
    readonly size?: Maybe<Scalars["String"]>;
    readonly soil?: Maybe<Scalars["String"]>;
    readonly sunlight?: Maybe<Scalars["String"]>;
    readonly watering?: Maybe<Scalars["String"]>;
    readonly minEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    readonly maxEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    readonly minLightLux?: Maybe<Scalars["Int"]>;
    readonly maxLightLux?: Maybe<Scalars["Int"]>;
    readonly minLightMmol?: Maybe<Scalars["Int"]>;
    readonly maxLightMmol?: Maybe<Scalars["Int"]>;
    readonly minSoilFertility?: Maybe<Scalars["Int"]>;
    readonly maxSoilFertility?: Maybe<Scalars["Int"]>;
    readonly minSoilHumidity?: Maybe<Scalars["Int"]>;
    readonly maxSoilHumidity?: Maybe<Scalars["Int"]>;
    readonly minTemperature?: Maybe<Scalars["Int"]>;
    readonly maxTemperature?: Maybe<Scalars["Int"]>;
};

export type AddSensorParameters = {
    readonly __typename?: "AddSensorParameters";
    readonly mACAddress?: Maybe<Scalars["String"]>;
    readonly name?: Maybe<Scalars["String"]>;
    readonly plantId?: Maybe<Scalars["Int"]>;
};

export type DeleteDeviceParameters = {
    readonly __typename?: "DeleteDeviceParameters";
    readonly id: Scalars["Int"];
};

export type DeletePlantParameters = {
    readonly __typename?: "DeletePlantParameters";
    readonly id: Scalars["Int"];
};

export type DeleteSensorParameters = {
    readonly __typename?: "DeleteSensorParameters";
    readonly id: Scalars["Int"];
};

export type Device = {
    readonly __typename?: "Device";
    readonly macAddress?: Maybe<Scalars["String"]>;
    readonly ipAddress?: Maybe<Scalars["String"]>;
    readonly port: Scalars["Int"];
    readonly name?: Maybe<Scalars["String"]>;
    readonly sensorDistances?: Maybe<ReadonlyArray<DeviceSensorDistance>>;
    readonly tags?: Maybe<ReadonlyArray<DeviceTag>>;
    readonly logs?: Maybe<ReadonlyArray<LogEntry>>;
    readonly id: Scalars["Int"];
    /** Contains all the failure times over the last 24h */
    readonly failuresLast24Hours?: Maybe<ReadonlyArray<DeviceError>>;
};

/** DeviceError */
export type DeviceError = {
    readonly __typename?: "DeviceError";
    readonly when: Scalars["Date"];
    readonly message?: Maybe<Scalars["String"]>;
};

/** Device Pagination */
export type DevicePagination = {
    readonly __typename?: "DevicePagination";
    /** collection of devices */
    readonly devices?: Maybe<ReadonlyArray<Device>>;
    /** total pages based on page size */
    readonly pageCount: Scalars["Int"];
};

export type DeviceSensorDistance = {
    readonly __typename?: "DeviceSensorDistance";
    readonly device?: Maybe<Device>;
    readonly sensor?: Maybe<Sensor>;
    readonly when: Scalars["Date"];
    readonly deviceId: Scalars["Int"];
    readonly sensorId: Scalars["Int"];
    readonly rssi?: Maybe<Scalars["Int"]>;
};

export enum DeviceSortField {
    MacAddress = "MACAddress",
    IpAddress = "IPAddress",
    Name = "Name",
}

export type DeviceTag = {
    readonly __typename?: "DeviceTag";
    readonly deviceId: Scalars["Int"];
    readonly device?: Maybe<Device>;
    readonly tag?: Maybe<Scalars["String"]>;
    readonly value?: Maybe<Scalars["String"]>;
};

export type EditDeviceParameters = {
    readonly __typename?: "EditDeviceParameters";
    readonly id: Scalars["Int"];
    readonly mACAddress?: Maybe<Scalars["String"]>;
    readonly iPAddress?: Maybe<Scalars["String"]>;
    readonly port: Scalars["Int"];
    readonly name?: Maybe<Scalars["String"]>;
};

export type EditPlantParameters = {
    readonly __typename?: "EditPlantParameters";
    readonly id: Scalars["Int"];
    readonly latinName?: Maybe<Scalars["String"]>;
    readonly alias?: Maybe<Scalars["String"]>;
    readonly display?: Maybe<Scalars["String"]>;
    readonly imageUrl?: Maybe<Scalars["String"]>;
    readonly blooming?: Maybe<Scalars["String"]>;
    readonly category?: Maybe<Scalars["String"]>;
    readonly color?: Maybe<Scalars["String"]>;
    readonly floralLanguage?: Maybe<Scalars["String"]>;
    readonly origin?: Maybe<Scalars["String"]>;
    readonly production?: Maybe<Scalars["String"]>;
    readonly fertilization?: Maybe<Scalars["String"]>;
    readonly pruning?: Maybe<Scalars["String"]>;
    readonly size?: Maybe<Scalars["String"]>;
    readonly soil?: Maybe<Scalars["String"]>;
    readonly sunlight?: Maybe<Scalars["String"]>;
    readonly watering?: Maybe<Scalars["String"]>;
    readonly minEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    readonly maxEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    readonly minLightLux?: Maybe<Scalars["Int"]>;
    readonly maxLightLux?: Maybe<Scalars["Int"]>;
    readonly minLightMmol?: Maybe<Scalars["Int"]>;
    readonly maxLightMmol?: Maybe<Scalars["Int"]>;
    readonly minSoilFertility?: Maybe<Scalars["Int"]>;
    readonly maxSoilFertility?: Maybe<Scalars["Int"]>;
    readonly minSoilHumidity?: Maybe<Scalars["Int"]>;
    readonly maxSoilHumidity?: Maybe<Scalars["Int"]>;
    readonly minTemperature?: Maybe<Scalars["Int"]>;
    readonly maxTemperature?: Maybe<Scalars["Int"]>;
};

export type EditSensorParameters = {
    readonly __typename?: "EditSensorParameters";
    readonly id: Scalars["Int"];
    readonly mACAddress?: Maybe<Scalars["String"]>;
    readonly name?: Maybe<Scalars["String"]>;
    readonly plantId?: Maybe<Scalars["Int"]>;
};

export type IdentityUser = {
    readonly __typename?: "IdentityUser";
    readonly id?: Maybe<Scalars["String"]>;
    readonly userName?: Maybe<Scalars["String"]>;
    readonly normalizedUserName?: Maybe<Scalars["String"]>;
    readonly email?: Maybe<Scalars["String"]>;
    readonly normalizedEmail?: Maybe<Scalars["String"]>;
    readonly emailConfirmed: Scalars["Boolean"];
    readonly securityStamp?: Maybe<Scalars["String"]>;
    readonly concurrencyStamp?: Maybe<Scalars["String"]>;
    readonly phoneNumber?: Maybe<Scalars["String"]>;
    readonly phoneNumberConfirmed: Scalars["Boolean"];
    readonly twoFactorEnabled: Scalars["Boolean"];
    readonly lockoutEnd?: Maybe<Scalars["DateTimeOffset"]>;
    readonly lockoutEnabled: Scalars["Boolean"];
    readonly accessFailedCount: Scalars["Int"];
};

export type LogEntry = {
    readonly __typename?: "LogEntry";
    readonly id: Scalars["Int"];
    readonly deviceId?: Maybe<Scalars["Int"]>;
    readonly device?: Maybe<Device>;
    readonly sensorId?: Maybe<Scalars["Int"]>;
    readonly sensor?: Maybe<Sensor>;
    readonly plantId?: Maybe<Scalars["Int"]>;
    readonly plant?: Maybe<Plant>;
    readonly userId?: Maybe<Scalars["String"]>;
    readonly user?: Maybe<IdentityUser>;
    readonly when: Scalars["Date"];
    readonly duration: Scalars["TimeSpan"];
    readonly event: LogEntryEvent;
    readonly result: LogEntryResult;
    readonly message?: Maybe<Scalars["String"]>;
};

export enum LogEntryEvent {
    Scan = "Scan",
    GetValues = "GetValues",
    GetFirmwareAndBattery = "GetFirmwareAndBattery",
    Add = "Add",
    Edit = "Edit",
    Delete = "Delete",
}

export enum LogEntryResult {
    Failed = "Failed",
    Successful = "Successful",
}

export type Mutation = {
    readonly __typename?: "Mutation";
    readonly addDevice?: Maybe<Device>;
    readonly editDevice?: Maybe<Device>;
    readonly deleteDevice?: Maybe<Device>;
    readonly scanForDevices?: Maybe<ReadonlyArray<Sensor>>;
    readonly addSensor?: Maybe<Sensor>;
    readonly editSensor?: Maybe<Sensor>;
    readonly deleteSensor?: Maybe<Sensor>;
    readonly addPlant?: Maybe<Plant>;
    readonly editPlant?: Maybe<Plant>;
    readonly deletePlant?: Maybe<Plant>;
};

export type MutationAddDeviceArgs = {
    mACAddress: Scalars["String"];
    iPAddress: Scalars["String"];
    port: Scalars["Int"];
    name: Scalars["String"];
};

export type MutationEditDeviceArgs = {
    id: Scalars["Int"];
    mACAddress: Scalars["String"];
    iPAddress: Scalars["String"];
    port: Scalars["Int"];
    name: Scalars["String"];
};

export type MutationDeleteDeviceArgs = {
    id: Scalars["Int"];
};

export type MutationAddSensorArgs = {
    mACAddress: Scalars["String"];
    name: Scalars["String"];
    plantId?: Maybe<Scalars["Int"]>;
};

export type MutationEditSensorArgs = {
    id: Scalars["Int"];
    mACAddress: Scalars["String"];
    name: Scalars["String"];
    plantId?: Maybe<Scalars["Int"]>;
};

export type MutationDeleteSensorArgs = {
    id: Scalars["Int"];
};

export type MutationAddPlantArgs = {
    latinName: Scalars["String"];
    alias: Scalars["String"];
    display: Scalars["String"];
    imageUrl: Scalars["String"];
    blooming?: Maybe<Scalars["String"]>;
    category?: Maybe<Scalars["String"]>;
    color?: Maybe<Scalars["String"]>;
    floralLanguage?: Maybe<Scalars["String"]>;
    origin?: Maybe<Scalars["String"]>;
    production?: Maybe<Scalars["String"]>;
    fertilization?: Maybe<Scalars["String"]>;
    pruning?: Maybe<Scalars["String"]>;
    size?: Maybe<Scalars["String"]>;
    soil?: Maybe<Scalars["String"]>;
    sunlight?: Maybe<Scalars["String"]>;
    watering?: Maybe<Scalars["String"]>;
    minEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    maxEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    minLightLux?: Maybe<Scalars["Int"]>;
    maxLightLux?: Maybe<Scalars["Int"]>;
    minLightMmol?: Maybe<Scalars["Int"]>;
    maxLightMmol?: Maybe<Scalars["Int"]>;
    minSoilFertility?: Maybe<Scalars["Int"]>;
    maxSoilFertility?: Maybe<Scalars["Int"]>;
    minSoilHumidity?: Maybe<Scalars["Int"]>;
    maxSoilHumidity?: Maybe<Scalars["Int"]>;
    minTemperature?: Maybe<Scalars["Int"]>;
    maxTemperature?: Maybe<Scalars["Int"]>;
};

export type MutationEditPlantArgs = {
    id: Scalars["Int"];
    latinName: Scalars["String"];
    alias: Scalars["String"];
    display: Scalars["String"];
    imageUrl: Scalars["String"];
    blooming?: Maybe<Scalars["String"]>;
    category?: Maybe<Scalars["String"]>;
    color?: Maybe<Scalars["String"]>;
    floralLanguage?: Maybe<Scalars["String"]>;
    origin?: Maybe<Scalars["String"]>;
    production?: Maybe<Scalars["String"]>;
    fertilization?: Maybe<Scalars["String"]>;
    pruning?: Maybe<Scalars["String"]>;
    size?: Maybe<Scalars["String"]>;
    soil?: Maybe<Scalars["String"]>;
    sunlight?: Maybe<Scalars["String"]>;
    watering?: Maybe<Scalars["String"]>;
    minEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    maxEnvironmentHumidity?: Maybe<Scalars["Int"]>;
    minLightLux?: Maybe<Scalars["Int"]>;
    maxLightLux?: Maybe<Scalars["Int"]>;
    minLightMmol?: Maybe<Scalars["Int"]>;
    maxLightMmol?: Maybe<Scalars["Int"]>;
    minSoilFertility?: Maybe<Scalars["Int"]>;
    maxSoilFertility?: Maybe<Scalars["Int"]>;
    minSoilHumidity?: Maybe<Scalars["Int"]>;
    maxSoilHumidity?: Maybe<Scalars["Int"]>;
    minTemperature?: Maybe<Scalars["Int"]>;
    maxTemperature?: Maybe<Scalars["Int"]>;
};

export type MutationDeletePlantArgs = {
    id: Scalars["Int"];
};

export type Plant = {
    readonly __typename?: "Plant";
    readonly latinName?: Maybe<Scalars["String"]>;
    readonly alias?: Maybe<Scalars["String"]>;
    readonly display?: Maybe<Scalars["String"]>;
    readonly imageUrl?: Maybe<Scalars["String"]>;
    readonly basic?: Maybe<PlantBasic>;
    readonly maintenance?: Maybe<PlantMaintenance>;
    readonly parameters?: Maybe<PlantParameters>;
    readonly id: Scalars["Int"];
};

export type PlantBasic = {
    readonly __typename?: "PlantBasic";
    readonly plantId: Scalars["Int"];
    readonly plant?: Maybe<Plant>;
    readonly blooming?: Maybe<Scalars["String"]>;
    readonly category?: Maybe<Scalars["String"]>;
    readonly color?: Maybe<Scalars["String"]>;
    readonly floralLanguage?: Maybe<Scalars["String"]>;
    readonly origin?: Maybe<Scalars["String"]>;
    readonly production?: Maybe<Scalars["String"]>;
};

export type PlantMaintenance = {
    readonly __typename?: "PlantMaintenance";
    readonly plantId: Scalars["Int"];
    readonly plant?: Maybe<Plant>;
    readonly fertilization?: Maybe<Scalars["String"]>;
    readonly pruning?: Maybe<Scalars["String"]>;
    readonly size?: Maybe<Scalars["String"]>;
    readonly soil?: Maybe<Scalars["String"]>;
    readonly sunlight?: Maybe<Scalars["String"]>;
    readonly watering?: Maybe<Scalars["String"]>;
};

export type PlantParameters = {
    readonly __typename?: "PlantParameters";
    readonly plantId: Scalars["Int"];
    readonly plant?: Maybe<Plant>;
    readonly environmentHumidity?: Maybe<Range>;
    readonly lightLux?: Maybe<Range>;
    readonly lightMmol?: Maybe<Range>;
    readonly soilFertility?: Maybe<Range>;
    readonly soilHumidity?: Maybe<Range>;
    readonly temperature?: Maybe<Range>;
};

export type Range = {
    readonly __typename?: "Range";
    readonly min?: Maybe<Scalars["Int"]>;
    readonly max?: Maybe<Scalars["Int"]>;
};

export type RootQuery = {
    readonly __typename?: "RootQuery";
    /** Return a Device by its Id */
    readonly device?: Maybe<Device>;
    /** Pagination. [defaults: page = 1, pageSize = 10, search = "", orderBy = "name", sortAsc = true] */
    readonly devicePager?: Maybe<DevicePagination>;
    readonly devices?: Maybe<ReadonlyArray<Device>>;
    readonly deviceSensorDistances?: Maybe<ReadonlyArray<DeviceSensorDistance>>;
    readonly devicesTags?: Maybe<ReadonlyArray<DeviceTag>>;
    readonly logEntries?: Maybe<ReadonlyArray<LogEntry>>;
    /** Return a LogEntry by its Id */
    readonly logEntry?: Maybe<LogEntry>;
    /** Return a Plant by its Id */
    readonly plant?: Maybe<Plant>;
    readonly plantBasics?: Maybe<ReadonlyArray<PlantBasic>>;
    readonly plantMaintenance?: Maybe<ReadonlyArray<PlantMaintenance>>;
    readonly plantParameters?: Maybe<ReadonlyArray<PlantParameters>>;
    readonly plants?: Maybe<ReadonlyArray<Plant>>;
    /** Return a Sensor by its Id */
    readonly sensor?: Maybe<Sensor>;
    readonly sensorBatteryReadings?: Maybe<
        ReadonlyArray<SensorBatteryAndVersionReading>
    >;
    readonly sensorDataReadings?: Maybe<ReadonlyArray<SensorDataReading>>;
    readonly sensors?: Maybe<ReadonlyArray<Sensor>>;
    readonly sensorTags?: Maybe<ReadonlyArray<SensorTag>>;
    readonly settings?: Maybe<ReadonlyArray<Setting>>;
    /** Return a IdentityUser by its Id */
    readonly user?: Maybe<IdentityUser>;
    readonly users?: Maybe<ReadonlyArray<IdentityUser>>;
};

export type RootQueryDeviceArgs = {
    id: Scalars["Int"];
};

export type RootQueryDevicePagerArgs = {
    page: Scalars["Int"];
    pageSize: Scalars["Int"];
    search?: Maybe<Scalars["String"]>;
    orderBy: DeviceSortField;
    sortOrder: SortOrder;
};

export type RootQueryLogEntryArgs = {
    id: Scalars["Int"];
};

export type RootQueryPlantArgs = {
    id: Scalars["Int"];
};

export type RootQuerySensorArgs = {
    id: Scalars["Int"];
};

export type RootQueryUserArgs = {
    id: Scalars["String"];
};

export type Sensor = {
    readonly __typename?: "Sensor";
    readonly mACAddress?: Maybe<Scalars["String"]>;
    readonly name?: Maybe<Scalars["String"]>;
    readonly plant?: Maybe<Plant>;
    readonly deviceDistances?: Maybe<ReadonlyArray<DeviceSensorDistance>>;
    readonly batteryAndVersionReadings?: Maybe<
        ReadonlyArray<SensorBatteryAndVersionReading>
    >;
    readonly dataReadings?: Maybe<ReadonlyArray<SensorDataReading>>;
    readonly tags?: Maybe<ReadonlyArray<SensorTag>>;
    readonly logs?: Maybe<ReadonlyArray<LogEntry>>;
    readonly id: Scalars["Int"];
};

export type SensorBatteryAndVersionReading = {
    readonly __typename?: "SensorBatteryAndVersionReading";
    readonly sensorId: Scalars["Int"];
    readonly sensor?: Maybe<Sensor>;
    readonly when: Scalars["Date"];
    readonly battery: Scalars["Int"];
    readonly version?: Maybe<Version>;
};

export type SensorDataReading = {
    readonly __typename?: "SensorDataReading";
    readonly sensorId: Scalars["Int"];
    readonly sensor?: Maybe<Sensor>;
    readonly when: Scalars["Date"];
    readonly moisture: Scalars["Int"];
    readonly brightness: Scalars["Int"];
    readonly temperature: Scalars["Float"];
    readonly conductivity: Scalars["Int"];
};

export type SensorTag = {
    readonly __typename?: "SensorTag";
    readonly sensorId: Scalars["Int"];
    readonly sensor?: Maybe<Sensor>;
    readonly tag?: Maybe<Scalars["String"]>;
    readonly value?: Maybe<Scalars["String"]>;
};

export type Setting = {
    readonly __typename?: "Setting";
    readonly key: Settings;
    readonly value?: Maybe<Scalars["String"]>;
    readonly lastChanged?: Maybe<Scalars["Date"]>;
};

export enum Settings {
    UpdateBatteryAndVersionCron = "UpdateBatteryAndVersionCron",
    UpdateValuesCron = "UpdateValuesCron",
    MqttClientId = "MQTTClientId",
    MqttServerAddress = "MQTTServerAddress",
    MqttPort = "MQTTPort",
    MqttUsername = "MQTTUsername",
    MqttPassword = "MQTTPassword",
    MqttUseTls = "MQTTUseTLS",
}

export enum SortOrder {
    Ascending = "Ascending",
    Descending = "Descending",
}

/** Information about subscriptions */
export type SubscriptionType = {
    readonly __typename?: "SubscriptionType";
    readonly name?: Maybe<Scalars["String"]>;
};

export type Version = {
    readonly __typename?: "Version";
    readonly major: Scalars["Int"];
    readonly minor: Scalars["Int"];
    readonly build: Scalars["Int"];
    readonly revision: Scalars["Int"];
    readonly majorRevision: Scalars["Int"];
    readonly minorRevision: Scalars["Int"];
};

export type ScanForDevicesMutationVariables = Exact<{ [key: string]: never }>;

export type ScanForDevicesMutation = { readonly __typename?: "Mutation" } & {
    readonly scanForDevices?: Maybe<
        ReadonlyArray<{ readonly __typename?: "Sensor" } & Pick<Sensor, "id">>
    >;
};

export type GetDevicesQueryVariables = Exact<{
    page: Scalars["Int"];
    pageSize: Scalars["Int"];
    search?: Maybe<Scalars["String"]>;
    orderBy: DeviceSortField;
    sortOrder: SortOrder;
}>;

export type GetDevicesQuery = { readonly __typename?: "RootQuery" } & {
    readonly devicePager?: Maybe<
        { readonly __typename?: "DevicePagination" } & Pick<
            DevicePagination,
            "pageCount"
        > & {
                readonly devices?: Maybe<
                    ReadonlyArray<
                        { readonly __typename?: "Device" } & Pick<
                            Device,
                            "id" | "name" | "ipAddress" | "macAddress"
                        > & {
                                readonly failuresLast24Hours?: Maybe<
                                    ReadonlyArray<
                                        {
                                            readonly __typename?: "DeviceError";
                                        } & Pick<
                                            DeviceError,
                                            "when" | "message"
                                        >
                                    >
                                >;
                                readonly tags?: Maybe<
                                    ReadonlyArray<
                                        {
                                            readonly __typename?: "DeviceTag";
                                        } & Pick<DeviceTag, "tag" | "value">
                                    >
                                >;
                            }
                    >
                >;
            }
    >;
};

export const ScanForDevicesDocument = gql`
    mutation scanForDevices {
        scanForDevices {
            id
        }
    }
`;

@Injectable({
    providedIn: "root",
})
export class ScanForDevicesGQL extends Apollo.Mutation<
    ScanForDevicesMutation,
    ScanForDevicesMutationVariables
> {
    document = ScanForDevicesDocument;
}
export const GetDevicesDocument = gql`
    query getDevices(
        $page: Int!
        $pageSize: Int!
        $search: String
        $orderBy: DeviceSortField!
        $sortOrder: SortOrder!
    ) {
        devicePager(
            page: $page
            pageSize: $pageSize
            search: $search
            orderBy: $orderBy
            sortOrder: $sortOrder
        ) {
            pageCount
            devices {
                id
                name
                ipAddress
                macAddress
                failuresLast24Hours {
                    when
                    message
                }
                tags {
                    tag
                    value
                }
            }
        }
    }
`;

@Injectable({
    providedIn: "root",
})
export class GetDevicesGQL extends Apollo.Query<
    GetDevicesQuery,
    GetDevicesQueryVariables
> {
    document = GetDevicesDocument;
}
