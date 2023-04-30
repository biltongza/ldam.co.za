import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ImageComponent } from './components/image/image.component';
import { PortfolioAlbumComponent } from './components/portfolio-album/portfolio-album.component';
import { imageMetadataResolver } from './imageMetadata.resolver';
import { manifestResolver } from './manifest.resolver';

const routes: Routes = [
  {
    path: '',
    resolve: {
      manifest: manifestResolver
    },
    children: [
      {
        path: '',
        component: PortfolioAlbumComponent
      },
      {
        path: 'image/:id',
        resolve: {
          metadata: imageMetadataResolver 
        },
        component: ImageComponent
      },
    ]
  },
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
