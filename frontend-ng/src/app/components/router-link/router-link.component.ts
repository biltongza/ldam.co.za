import { Component, Input } from '@angular/core';
import { RouterLinkActive, RouterLink } from '@angular/router';

@Component({
  selector: 'app-router-link',
  templateUrl: './router-link.component.html',
  styleUrls: ['./router-link.component.scss'],
  standalone: true,
  imports: [RouterLinkActive, RouterLink]
})
export class RouterLinkComponent {
  matches = false;
  @Input() path = '';
}
