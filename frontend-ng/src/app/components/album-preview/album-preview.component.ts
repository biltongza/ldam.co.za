import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";
import { Album, ImageMetadata } from "src/app/types";

@Component({
  selector: "app-album-preview",
  templateUrl: "./album-preview.component.html",
  styleUrls: ["./album-preview.component.scss"],
})
export class AlbumPreviewComponent implements OnChanges {
  @Input() album?: Album;
  @Input() numberOfImages: number = Number.MAX_SAFE_INTEGER;

  images?: ImageMetadata[];

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes["album"] || changes["numberOfImages"]) && this.album) {
      const entries = Object.entries(this.album.images).map(([, meta]) => meta);
      this.images = entries
        .sort((meta1, meta2) => {
          const a = new Date(meta1.captureDate).valueOf();
          const b = new Date(meta2.captureDate).valueOf();

          return Number(b > a) - Number(b < a);
        })
        .slice(0, this.numberOfImages);
    }
  }

  getClassList(image: ImageMetadata): string {
    const parts = image.aspectRatio.split(":").map((part) => +part);
    const isLandscape = parts[0] > parts[1];
    return `thumbnail ${isLandscape ? "span-col" : ""}`;
  }
}
