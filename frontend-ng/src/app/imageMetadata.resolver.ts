import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, Router } from '@angular/router';
import { EMPTY, of } from 'rxjs';
import { getRouteData } from './getRouteParam.function';
import { ImageMetadata, Manifest } from './types';

export const imageMetadataResolver: ResolveFn<ImageMetadata> = (route: ActivatedRouteSnapshot) => {
	const router = inject(Router);
	const id = route.params['id'];
	const manifest: Manifest = getRouteData(route, 'manifest');

	const allImages = manifest.albums
		.flatMap((x) => Object.entries(x.images))
		.map(([_, metadata]) => metadata);

	const match = allImages.find((x) => x.id === id);
	if (!match) {
		router.navigate(['/not-found']);
		return EMPTY;
	}
	return of(match);
};
