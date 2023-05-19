import { NgIf } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HighResHref, HighResMaxDimension, StorageBaseUrl } from 'src/app/consts';
import { TitleService } from 'src/app/services/title.service';
import { ImageMetadata } from 'src/app/types';

@Component({
	selector: 'app-image',
	templateUrl: './image.component.html',
	styleUrls: ['./image.component.scss'],
	standalone: true,
	imports: [NgIf],
	schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ImageComponent implements OnInit, OnDestroy {
	src: string;
	metadata: ImageMetadata;
	date: Date;

	constructor(route: ActivatedRoute, private readonly titleService: TitleService) {
		this.metadata = route.snapshot.data['metadata'];

		const href = this.metadata.hrefs[HighResHref];
		this.src = `${StorageBaseUrl}/${href}.jpg`;

		const [widthRatio, heightRatio] = this.metadata.aspectRatio.split(':').map((x) => Number(x));

		this.date = new Date(this.metadata.captureDate);
		const width =
			widthRatio > heightRatio
				? HighResMaxDimension
				: (widthRatio / heightRatio) * HighResMaxDimension;
		const height =
			heightRatio > widthRatio
				? HighResMaxDimension
				: (heightRatio / widthRatio) * HighResMaxDimension;
	}
	ngOnInit(): void {
		this.titleService.setTitle(this.metadata.title);
	}
	ngOnDestroy(): void {
		this.titleService.clearTitle();
	}
}
