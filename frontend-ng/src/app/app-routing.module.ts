import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AboutComponent } from "./components/about/about.component";
import { CollectionsComponent } from './components/collections/collections.component';
import { ErrorComponent } from './components/error/error.component';
import { ImageComponent } from "./components/image/image.component";
import { PortfolioAlbumComponent } from "./components/portfolio-album/portfolio-album.component";
import { imageMetadataResolver } from "./imageMetadata.resolver";
import { manifestResolver } from "./manifest.resolver";

const routes: Routes = [
  {
    path: "",
    resolve: {
      manifest: manifestResolver,
    },
    children: [
      {
        path: "",
        component: PortfolioAlbumComponent,
      },
      {
        path: "image/:id",
        resolve: {
          metadata: imageMetadataResolver,
        },
        component: ImageComponent,
      },
      {
        path: 'collections',
        component: CollectionsComponent,
      },
      {
        path: "about",
        component: AboutComponent,
      },
    ],
  },
  {
    path: '**',
    component: ErrorComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
