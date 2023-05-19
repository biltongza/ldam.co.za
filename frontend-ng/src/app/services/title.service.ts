import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class TitleService {
  private currentTitle?: string;
  constructor(private readonly title: Title) {
    this.update();
  }

  setTitle(text?: string) {
    this.currentTitle = text;
    this.update();
  }

  clearTitle() {
    this.currentTitle = undefined;
    this.update();
  }

  private update() {
    this.title.setTitle(
      `${
        this.currentTitle ? this.currentTitle + ' - ' : ''
      }Logan Dam - Software Engineer, Photographer`
    );
  }
}
