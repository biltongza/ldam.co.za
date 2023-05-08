import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { getRouteData } from 'src/app/getRouteParam.function';
import { Album, Manifest } from 'src/app/types';
import { AlbumPreviewComponent } from '../album-preview/album-preview.component';
import { NgFor } from '@angular/common';

@Component({
	selector: 'app-collections',
	templateUrl: './collections.component.html',
	styleUrls: ['./collections.component.scss'],
	standalone: true,
	imports: [NgFor, AlbumPreviewComponent]
})
export class CollectionsComponent {
	albums: Album[];
	constructor(route: ActivatedRoute) {
		const manifest: Manifest = getRouteData(route.snapshot, 'manifest');
		this.albums = manifest.albums.filter((x) => !x.isPortfolio);
	}
}
