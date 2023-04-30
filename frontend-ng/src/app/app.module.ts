import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";

import { HttpClientModule } from "@angular/common/http";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { AlbumPreviewComponent } from "./components/album-preview/album-preview.component";
import { FooterComponent } from "./components/footer/footer.component";
import { HeaderComponent } from "./components/header/header.component";
import { RouterLinkComponent } from "./components/router-link/router-link.component";
import { SocialIconComponent } from "./components/social-icon/social-icon.component";
import { ThumbnailComponent } from "./components/thumbnail/thumbnail.component";
import { PortfolioAlbumComponent } from './components/portfolio-album/portfolio-album.component';
import { ImageComponent } from './components/image/image.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    RouterLinkComponent,
    SocialIconComponent,
    FooterComponent,
    AlbumPreviewComponent,
    ThumbnailComponent,
    PortfolioAlbumComponent,
    ImageComponent
  ],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule],
  providers: [],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AppModule {}
