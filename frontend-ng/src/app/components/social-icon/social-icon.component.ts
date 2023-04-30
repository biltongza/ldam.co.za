import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-social-icon',
  templateUrl: './social-icon.component.html',
  styleUrls: ['./social-icon.component.scss']
})
export class SocialIconComponent {
	@Input() text: string = '';
	@Input() label: string = '';
	@Input() icon: string = '';
	@Input() href: string = ''
  @Input() showText: boolean = false;
}
