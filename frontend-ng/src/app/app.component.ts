import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from './components/footer/footer.component';
import { HeaderComponent } from './components/header/header.component';
import { TitleService } from './services/title.service';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss'],
	standalone: true,
	imports: [HeaderComponent, RouterOutlet, FooterComponent]
})
export class AppComponent {
	constructor(titleService: TitleService) {
		titleService.clearTitle();
	}
	title = 'frontend-ng';
}
