import { Component } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { HighResHref, HighResMaxDimension, StorageBaseUrl } from 'src/app/consts';
import { ImageMetadata } from "src/app/types";

@Component({
  selector: "app-image",
  templateUrl: "./image.component.html",
  styleUrls: ["./image.component.scss"],
})
export class ImageComponent {
  src: string;
  metadata: ImageMetadata;
  date: Date;

  constructor(route: ActivatedRoute) {
    this.metadata = route.snapshot.data["metadata"];

    const href = this.metadata.hrefs[HighResHref];
    this.src = `${StorageBaseUrl}/${href}.jpg`;

    const [widthRatio, heightRatio] = this.metadata.aspectRatio
      .split(":")
      .map((x) => Number(x));

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
}
