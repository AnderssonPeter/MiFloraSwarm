import { PipeTransform, Pipe } from '@angular/core';

@Pipe({name: 'range'})
export class RangePipe implements PipeTransform {
  transform(value: number, start: number = 0): number[] {
    return Array.from({length: value - start + 1},(v, k) => k + start);
  }
}