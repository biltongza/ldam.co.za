import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { StorageBaseUrl, ThumbnailHrefNormalDensity } from 'src/app/consts';
import { ImageMetadata } from 'src/app/types';
import { NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';

const thumbnailSizes = {
  thumbnail2x: '320w',
  '640': '640w',
  '1280': '1280w'
};

// order is important here apparently
const formats = {
  webp: 'image/webp',
  jpg: 'image/jpeg'
};

@Component({
  selector: 'app-thumbnail',
  templateUrl: './thumbnail.component.html',
  styleUrls: ['./thumbnail.component.scss'],
  standalone: true,
  imports: [RouterLink, NgFor]
})
export class ThumbnailComponent implements OnChanges {
  @Input() image?: ImageMetadata = undefined;
  imageRoute?: string;

  src?: string;
  srcSets: { srcSet: string; mime: string }[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['image'] && this.image) {
      this.imageRoute = `/image/${this.image.id}`;
      this.src = `${StorageBaseUrl}/${this.image.hrefs[ThumbnailHrefNormalDensity]}.webp`;
      this.srcSets = Object.entries(thumbnailSizes).flatMap(([sizeKey, maxWidth]) =>
        Object.entries(formats).map(([extension, mime]) => ({
          srcSet: `${StorageBaseUrl}/${this.image!.hrefs[sizeKey]}.${extension} ${maxWidth}`,
          mime
        }))
      );
    }
  }
}
