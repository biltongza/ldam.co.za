import { NgFor, NgIf } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
import { RouterLinkComponent } from '../router-link/router-link.component';
import { SocialIconComponent } from '../social-icon/social-icon.component';

@Component({
	selector: 'app-header',
	templateUrl: './header.component.html',
	styleUrls: ['./header.component.scss'],
	standalone: true,
	imports: [NgIf, RouterLinkComponent, NgFor, SocialIconComponent],
	schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class HeaderComponent {
	smol = false;
	expanded = false;

	routes = [
		{ label: 'About', path: '/about' },
		{ label: 'Collections', path: '/collections' }
		// TODO: reinstate when blogs are on an API
		//{ label: 'Blog', path: '/blog' }
	];

	socials = [
		{ text: 'GitHub', label: 'github', href: 'https://github.com/biltongza', icon: 'github' },
		{
			text: 'LinkedIn',
			label: 'linkedin',
			href: 'https://www.linkedin.com/in/logan-dam/',
			icon: 'linkedin'
		},
		{
			text: 'Instagram',
			label: 'instagram',
			href: 'https://www.instagram.com/thebiltong/',
			icon: 'instagram'
		},
		{ text: 'Twitter', label: 'twitter', href: 'https://twitter.com/TheBiltong', icon: 'twitter' }
	];

	toggle() {
		this.expanded = !this.expanded;
	}
}
