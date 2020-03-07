import { Component, OnInit } from '@angular/core';
import { faCogs, faHdd, faEye, faMicrochip, faSeedling, faAngleDoubleRight, faAngleDoubleLeft, faMagic, faInfoCircle, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { SizeProp } from '@fortawesome/fontawesome-svg-core';

export type RouteDate = { label, icon }

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
    { icon: faEye, label: 'Overview', route: '/overview' },
    { icon: faMicrochip, label: 'Devices', route: '/devices' },
    { icon: faMagic, label: 'Sensors', route: '/sensors' },
    { icon: faSeedling, label: 'Plants', route: '/plants' },
    { icon: faCogs, label: 'Settings', route: '/settings' },
    { icon: faHdd, label: 'Tasks', route: '/tasks' },
    { icon: faInfoCircle, label: 'About', route: '/about' }
  ];
  currentPage = { label: '', icon: null };
  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.currentPage = activatedRoute.snapshot.children[0].data as RouteDate;
      }
    });
  }

  ngOnInit(): void {
  }

  

  toggle() {
    this.expanded = !this.expanded;
  }

}
