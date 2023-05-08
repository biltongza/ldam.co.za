import { Component } from '@angular/core';
import { FooterComponent } from './components/footer/footer.component';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss'],
	standalone: true,
	imports: [HeaderComponent, RouterOutlet, FooterComponent]
})
export class AppComponent {
	title = 'frontend-ng';
}
