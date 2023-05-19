import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from './components/footer/footer.component';
import { HeaderComponent } from './components/header/header.component';
import { MetadataService } from './services/metadata.service';
import { TitleService } from './services/title.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: true,
  imports: [HeaderComponent, RouterOutlet, FooterComponent]
})
export class AppComponent {
  constructor(titleService: TitleService, metadataService: MetadataService) {
    titleService.clearTitle();
    metadataService.clearMetadata();
  }
  title = 'frontend-ng';
}
