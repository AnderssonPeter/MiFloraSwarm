import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { LayoutComponent } from './layout/layout.component';
import { SettingsComponent } from './settings/settings.component';
import { faCogs, faEye, faMicrochip, faMagic, faSeedling, faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { OverviewComponent } from './overview/overview.component';
import { DevicesComponent } from './devices/devices.component';
import { SensorsComponent } from './sensors/sensors.component';
import { PlantsComponent } from './plants/plants.component';
import { AboutComponent } from './about/about.component';
import { TasksComponent } from './tasks/tasks.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    data: {
      hidden: true
    }
  },
  { 
    path: '', 
    component: LayoutComponent, 
    children: [
      {
        path: 'overview',
        component: OverviewComponent,
        data: {
          label: 'Overview',
          icon: faEye
        }
      },
      {
        path: 'devices',
        component: DevicesComponent,
        data: {
          label: 'Devices',
          icon: faMicrochip
        }
      },
      {
        path: 'sensors',
        component: SensorsComponent,
        data: {
          label: 'Sensors',
          icon: faMagic
        }
      },
      {
        path: 'plants',
        component: PlantsComponent,
        data: {
          label: 'Plants',
          icon: faSeedling
        }
      },
      { 
        path: 'settings', 
        component: SettingsComponent,
        data: {
          label: 'Settings',
          icon: faCogs
        }
      },
      {
        path: 'tasks',
        component: TasksComponent,
        data: {
          label: 'Tasks',
          icon: faMagic
        }
      },
      {
        path: 'about',
        component: AboutComponent,
        data: {
          label: 'About',
          icon: faInfoCircle
        }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
