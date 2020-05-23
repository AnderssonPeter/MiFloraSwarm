import { Component, OnInit } from '@angular/core';
import { faCogs, faHdd, faEye, faMicrochip, faSeedling, faAngleDoubleRight, faAngleDoubleLeft, faMagic, faInfoCircle, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { ActivatedRoute, Router, NavigationEnd, NavigationStart, NavigationCancel, NavigationError } from '@angular/router';

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
  loading = false;
  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    
    router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.loading = true;
        console.log('start');
      }
      else if (event instanceof NavigationEnd) {
        this.loading = false;
        console.log('end');
        this.currentPage = activatedRoute.snapshot.children[0].data as RouteDate;
      }
      else if (event instanceof NavigationCancel ||
               event instanceof NavigationError) {
        console.log('cancel');
        this.loading = false;
      }
    });
  }

  ngOnInit(): void {
  }

  

  toggle() {
    this.expanded = !this.expanded;
  }

}
