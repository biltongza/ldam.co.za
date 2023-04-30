import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Album, Manifest } from 'src/app/types';

@Component({
  selector: 'app-portfolio-album',
  templateUrl: './portfolio-album.component.html',
  styleUrls: ['./portfolio-album.component.scss']
})
export class PortfolioAlbumComponent {
  portfolio: Album;
  constructor(route: ActivatedRoute) {
    const manifest = route.snapshot.data['manifest'] as Manifest;
    this.portfolio = manifest.albums.find(x => x.isPortfolio)!;
  }
}
