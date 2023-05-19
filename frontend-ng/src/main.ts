import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { setBasePath } from '@shoelace-style/shoelace/dist/utilities/base-path';
import { AppRoutingModule } from './app/app-routing.module';
import { AppComponent } from './app/app.component';

setBasePath('https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.4.0/dist/');

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(BrowserModule, AppRoutingModule),
    provideHttpClient(withInterceptorsFromDi())
  ]
}).catch((err) => console.error(err));
