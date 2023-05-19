import { Injectable } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { defaultMetadata } from '../consts';

@Injectable({
  providedIn: 'root'
})
export class MetadataService {
  private _currentMetas: { [key: string]: string } = {};
  constructor(private readonly meta: Meta) {}

  updateMetadata(metadata?: { [key: string]: string }) {
    const metas: { [key: string]: string } = {
      ...defaultMetadata,
      ...(metadata || {})
    };

    // look for any existing meta tags
    for (const existingKey of Object.keys(this._currentMetas)) {
      if (!metas[existingKey]) {
        this.meta.removeTag(this.getMetaAttribute(existingKey));
        continue;
      } else if (metas[existingKey] !== this._currentMetas[existingKey]) {
        this.meta.updateTag({ property: existingKey, content: metas[existingKey] });
        continue;
      }
    }

    // handle new tags
    for (const newKey of Object.keys(metas)) {
      if (this._currentMetas[newKey]) {
        continue;
      }

      this.meta.addTag({ property: newKey, content: metas[newKey] });
    }
    this._currentMetas = metas;
  }

  clearMetadata() {
    this.updateMetadata({});
  }

  private getMetaAttribute(key: string) {
    return `property='${key}'`;
  }
}
