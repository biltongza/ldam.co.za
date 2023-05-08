import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';



import { registerIconLibrary } from '@shoelace-style/shoelace';
import { setBasePath } from '@shoelace-style/shoelace/dist/utilities/base-path';
import { importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/app.component';
import { withInterceptorsFromDi, provideHttpClient } from '@angular/common/http';
import { AppRoutingModule } from './app/app-routing.module';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';

setBasePath('https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.4.0/dist/');

registerIconLibrary('ionicons', {
  resolver: name => `https://cdn.jsdelivr.net/npm/ionicons@5.1.2/dist/ionicons/svg/${name}.svg`,
  mutator: svg => {
    svg.setAttribute('fill', 'currentColor');
    svg.setAttribute('stroke', 'currentColor');
    Array.from(svg.querySelectorAll('.ionicon-fill-none')).map(el => el.setAttribute('fill', 'none'));
    Array.from(svg.querySelectorAll('.ionicon-stroke-width')).map(el => el.setAttribute('stroke-width', '32px'));
  }
});

bootstrapApplication(AppComponent, {
    providers: [
        importProvidersFrom(BrowserModule, AppRoutingModule),
        provideHttpClient(withInterceptorsFromDi())
    ]
})
  .catch(err => console.error(err));
