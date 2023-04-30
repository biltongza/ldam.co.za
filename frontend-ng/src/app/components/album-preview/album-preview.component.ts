import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Album, ImageMetadata } from 'src/app/types';

@Component({
  selector: 'app-album-preview',
  templateUrl: './album-preview.component.html',
  styleUrls: ['./album-preview.component.scss']
})
export class AlbumPreviewComponent implements OnChanges {
  
  @Input() album?: Album;
	@Input() numberOfImages: number = Number.MAX_SAFE_INTEGER;

  images?: ImageMetadata[];

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['album'] && this.album) {
      this.images = Object.entries(this.album.images).map(([_, x]) => x);
    }
  }

  getClassList(image: ImageMetadata): string {
    const parts = image.aspectRatio.split(':').map((part) => +part);
    const isLandscape = parts[0] > parts[1];
    return `thumbnail ${isLandscape ? 'span-col' : ''}`;
  }
  
}