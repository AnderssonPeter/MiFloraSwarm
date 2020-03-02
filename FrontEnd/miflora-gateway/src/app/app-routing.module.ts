import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { LayoutComponent } from './layout/layout.component';
import { SettingsComponent } from './settings/settings.component';
import { faCogs } from '@fortawesome/free-solid-svg-icons';

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
        path: 'settings', 
        component: SettingsComponent,
        data: {
          icon: faCogs
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
