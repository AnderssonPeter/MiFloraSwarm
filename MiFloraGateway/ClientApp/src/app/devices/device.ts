import { ApexAxisChartSeries } from 'ng-apexcharts';


export class DataLine {
    constructor(
        public readonly name: string,
        public readonly color: string,
        public readonly values: DataPoint[],
        public readonly range?: Span) {
    }

    public toApexAxisChartSeries() {
        var apexDataPoints = this.values.map(dataPoint => ({
            x: dataPoint.when,
            y: dataPoint.value,
            fillColor: this.color,
            strokeColor: this.color
        }));
        return {
            name: this.name,
            data: apexDataPoints
        };
    };
}

export class Device {
    constructor(public readonly name: string, 
                public readonly failureRate: DataLine,
                public readonly macAddress: string,
                public readonly ipAddress: string, 
                public readonly type: string,
                public readonly version: string,
                public readonly tags: string[]) {
    }
}

export interface DataPoint {
    readonly when: Date;
    readonly value: number;
}

export function getDummyDataLine(name: string, color: string, count: number, intervalInMinutes: number, minValue: number, maxValue: number, round: boolean) {
    return new DataLine(name, color, getDummyDataPoints(count, intervalInMinutes, minValue, maxValue, round), { minValue: minValue, maxValue: maxValue });
}

export function getDummyDataPoints(count: number, intervalInMinutes: number, minValue: number, maxValue: number, round: boolean) {
    const baseDate = Date.now();
    const MS_PER_MINUTE = 60 * 1000;
    const diff = maxValue - minValue;
    const fix = (value: number) => round ? Math.round(value) : value;
    
    return [...Array(count).keys()].map(x => ({ when: new Date(baseDate - x * MS_PER_MINUTE * intervalInMinutes), value: fix(Math.random() * diff + minValue) }));
}

export class Span<T = number> {
    constructor(public readonly minValue?: T, public readonly maxValue?: T) {
    }
}

export class Sensor {
    constructor(public readonly name: string, 
                public readonly macAddress: string,
                public readonly plant: Plant,
                public readonly moisture: DataLine,
                public readonly conductivity: DataLine,
                public readonly temperature: DataLine,
                public readonly brightness: DataLine,
                public readonly battery: number) {

                }
}

export class Plant {
    constructor(public readonly name: string,
                public readonly latinName: string,
                public readonly moisture: Span,
                public readonly conductivity: Span,
                public readonly temperature: Span,
                public readonly brightness: Span) {
    }

}