import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { map } from 'rxjs';
import { StorageBaseUrl } from './consts';
import { Manifest } from './types';

export const manifestResolver: ResolveFn<Manifest> = () => {
  const httpClient = inject(HttpClient);
  return httpClient.get(`${StorageBaseUrl}/manifest.json`).pipe(map((res) => res as Manifest));
};
