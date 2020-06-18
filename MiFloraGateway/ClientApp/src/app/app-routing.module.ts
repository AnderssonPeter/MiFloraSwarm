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
import { DeviceResolverService } from './devices/device-resolver.service';
import { AuthenticationGuard } from './services/authentication.guard';
import { OnboardingComponent } from './onboarding/onboarding.component';
import { OnboardingGuard } from './onboarding/onboarding.guard';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [OnboardingGuard],
    data: {
      hidden: true
    }
  },
  {
    path: 'onboarding',
    component: OnboardingComponent,
    data: {
      hidden: true
    }
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthenticationGuard],
    children: [
      {
        path: '',
        redirectTo: 'overview',
        pathMatch: 'full'
      },
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
        resolve: {
          devices: DeviceResolverService
        },
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
