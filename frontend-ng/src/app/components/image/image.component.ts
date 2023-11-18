import { CUSTOM_ELEMENTS_SCHEMA, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HighResHref, HighResMaxDimension, StorageBaseUrl } from 'src/app/consts';
import { MetadataService } from 'src/app/services/metadata.service';
import { TitleService } from 'src/app/services/title.service';
import { ImageMetadata } from 'src/app/types';

@Component({
  selector: 'app-image',
  templateUrl: './image.component.html',
  styleUrls: ['./image.component.scss'],
  standalone: true,
  imports: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ImageComponent implements OnInit, OnDestroy {
  src: string;
  metadata: ImageMetadata;
  date: Date;

  constructor(
    route: ActivatedRoute,
    private readonly titleService: TitleService,
    private readonly metaService: MetadataService
  ) {
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

    this.metaService.updateMetadata({
      'twitter:card': 'summary_large_image',
      'og:type': 'article',
      'og:image': this.src,
      'og:title': this.metadata.title,
      'og:description': 'Photography by Logan Dam',
      'og:image:width': width.toString(),
      'og:image:height': height.toString(),
      'og:image:alt': this.metadata.caption
    });
  }
  ngOnInit(): void {
    this.titleService.setTitle(this.metadata.title);
  }
  ngOnDestroy(): void {
    this.titleService.clearTitle();
    this.metaService.clearMetadata();
  }
}
