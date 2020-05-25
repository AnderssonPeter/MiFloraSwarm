import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Device } from './device';
import { faSearch, faMicrochip, faSatelliteDish, faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss']
})
export class DevicesComponent implements OnInit {
  public devices?: Device[];
  icons = {
    search: faSearch,
    icon: faMicrochip,
    scan: faSatelliteDish, //faSync
    edit: faEdit,
    remove: faTrash
  }
  constructor(private route: ActivatedRoute) {
    console.log('constructor');
  }

  ngOnInit(): void {
    console.log('ngOnInit');
    this.route.data.subscribe(params => {
      console.log('data');
      this.devices = params.devices;
    })
  }

}
