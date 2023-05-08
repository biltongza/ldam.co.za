import { CUSTOM_ELEMENTS_SCHEMA, Component, Input } from '@angular/core';

@Component({
	selector: 'app-social-icon',
	templateUrl: './social-icon.component.html',
	styleUrls: ['./social-icon.component.scss'],
	standalone: true,
	schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class SocialIconComponent {
	@Input() text = '';
	@Input() label = '';
	@Input() icon = '';
	@Input() href = '';
	@Input() showText = false;
}
