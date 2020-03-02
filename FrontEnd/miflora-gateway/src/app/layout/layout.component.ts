import { Component, OnInit } from '@angular/core';
import { faCogs, faHdd, faEye, faMicrochip, faSeedling, faAngleDoubleRight, faAngleDoubleLeft, faMagic, faInfoCircle, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  expanded: boolean = false
  icons = {
    expand: faAngleDoubleRight,
    contract: faAngleDoubleLeft,
    logout: faSignOutAlt
  }
  menuItems = [
    { icon: faEye, label: 'Overview', route: '' },
    { icon: faMicrochip, label: 'Devices', route: '' },
    { icon: faMagic, label: 'Sensors', route: '' },
    { icon: faSeedling, label: 'Plants', route: '' },
    { icon: faCogs, label: 'Settings', route: '' },
    { icon: faHdd, label: 'Tasks', route: '' },
    { icon: faInfoCircle, label: 'About', route: '' }
  ]

  toggle() {
    this.expanded = !this.expanded;
  }

  constructor() { }

  ngOnInit(): void {
  }

}
