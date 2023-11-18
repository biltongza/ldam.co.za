import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { getRouteData } from 'src/app/getRouteParam.function';
import { TitleService } from 'src/app/services/title.service';
import { Album, Manifest } from 'src/app/types';
import { AlbumPreviewComponent } from '../album-preview/album-preview.component';

@Component({
  selector: 'app-collections',
  templateUrl: './collections.component.html',
  styleUrls: ['./collections.component.scss'],
  standalone: true,
  imports: [AlbumPreviewComponent]
})
export class CollectionsComponent implements OnInit, OnDestroy {
  albums: Album[];
  constructor(
    route: ActivatedRoute,
    private readonly titleService: TitleService
  ) {
    const manifest: Manifest = getRouteData(route.snapshot, 'manifest');
    this.albums = manifest.albums.filter((x) => !x.isPortfolio);
  }
  ngOnInit(): void {
    this.titleService.setTitle('Collections');
  }
  ngOnDestroy(): void {
    this.titleService.clearTitle();
  }
}
