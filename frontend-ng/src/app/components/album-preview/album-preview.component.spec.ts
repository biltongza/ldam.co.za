import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AlbumPreviewComponent } from './album-preview.component';

describe('AlbumPreviewComponent', () => {
  let component: AlbumPreviewComponent;
  let fixture: ComponentFixture<AlbumPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [AlbumPreviewComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(AlbumPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
