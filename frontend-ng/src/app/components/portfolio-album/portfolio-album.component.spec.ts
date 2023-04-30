import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioAlbumComponent } from './portfolio-album.component';

describe('PortfolioAlbumComponent', () => {
  let component: PortfolioAlbumComponent;
  let fixture: ComponentFixture<PortfolioAlbumComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PortfolioAlbumComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortfolioAlbumComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
