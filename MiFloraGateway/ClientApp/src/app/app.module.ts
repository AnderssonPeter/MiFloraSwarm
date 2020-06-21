import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { LoginComponent } from './login/login.component';
import { LayoutComponent } from './layout/layout.component';
import { SettingsComponent } from './settings/settings.component';
import { OverviewComponent } from './overview/overview.component';
import { DevicesComponent } from './devices/devices.component';
import { SensorsComponent } from './sensors/sensors.component';
import { PlantsComponent } from './plants/plants.component';
import { AboutComponent } from './about/about.component';
import { TasksComponent } from './tasks/tasks.component';
import { ChartComponent } from './chart/chart.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { API_BASE_URL, AuthenticationClient, OnboardingClient } from './api/rest/rest.client';
import { OnboardingComponent } from './onboarding/onboarding.component';
import { DynamicFormComponent} from './dynamic.form/dynamic.form.component';
import { DialogComponent } from './dialog/dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    LayoutComponent,
    SettingsComponent,
    OverviewComponent,
    DevicesComponent,
    SensorsComponent,
    PlantsComponent,
    AboutComponent,
    TasksComponent,
    ChartComponent,
    OnboardingComponent,
    DynamicFormComponent,
    DialogComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FontAwesomeModule,
    FormsModule,
    ReactiveFormsModule,
    NgApexchartsModule
  ],
  providers: [
    { provide: API_BASE_URL, useValue: '' },
    AuthenticationClient,
    OnboardingClient
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor() {
  }
}