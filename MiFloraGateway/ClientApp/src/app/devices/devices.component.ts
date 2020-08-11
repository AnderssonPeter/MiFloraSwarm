import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Device } from './device';
import { faSearch, faMicrochip, faSatelliteDish, faEdit, faTrash, faSortUp, faSortDown } from '@fortawesome/free-solid-svg-icons';
import { DeviceSortField, ScanForDevicesGQL } from '../api/graphql/graphql.client';
import { Paginator } from "../pagination/Paginator";

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss']
})
export class DevicesComponent implements OnInit {
    public readonly DeviceSortField = DeviceSortField;
    public devicePaginator!: Paginator<Device, DeviceSortField>;
    public isScanning: boolean = false;
    icons = {
        search: faSearch,
        icon: faMicrochip,
        scan: faSatelliteDish, //faSync
        edit: faEdit,
        remove: faTrash,
        sortDown: faSortDown,
        sortUp: faSortUp
    }

    constructor(private readonly route: ActivatedRoute, private readonly scanForDevicesGQL: ScanForDevicesGQL) {

    }

    ngOnInit(): void {
        console.log('ngOnInit');
        this.route.data.subscribe(params => {
            console.log('data');
            this.devicePaginator = params.devicePaginator;
        })
    }

    public async scan() {
        //this logic should be moved into a diffrent class and not be in the controller
        if (this.isScanning) {
            return;
        }
        this.isScanning = true;
        const ids = await this.scanForDevicesGQL.mutate().toPromise();
        this.isScanning = false;
        this.devicePaginator.update();
    }
}
