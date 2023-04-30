import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  smol = false;
  expanded = false;

  routes = [
		{ label: 'About', path: '/about' },
		{ label: 'Collections', path: '/collections' },
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
