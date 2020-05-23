import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Device, DataLine, Span, DataPoint,  getDummyDataLine } from './device';
import { Observable, from } from 'rxjs';
import { delay } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class DeviceResolverService implements Resolve<Device[]> {

  constructor() { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Device[] | Observable<Device[]> | Promise<Device[]> {
    return from([[new Device('Garage', getDummyDataLine('Failure rate', '#FF0000', 24, 30, 0, 1, true), '38-2C-4A-B5-A1-DD', '192.168.0.156', 'ESP32', '0.1', ['Garage']),
                  new Device('Livingroom', getDummyDataLine('Failure rate', '#FF0000', 24, 30, 0, 1, true), '38-2C-4A-B5-A1-BB', '192.168.0.157', 'PC (Docker)', '0.2', ['Livingroom'])]])
        .pipe(delay(2000));
  }
}
