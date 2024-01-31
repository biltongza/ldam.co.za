import { metaStore, titleStore } from '$lib/stores';
import type { ImageMetadata } from '$lib/types';
import { HighResHref, HighResMaxDimension, StorageBaseUrl } from '$lib/__consts';
import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

let src: string;
let metadata: ImageMetadata;
let date: Date;
export const load: PageLoad = async function ({ params, parent }) {
	const { manifest } = await parent();
	const imageId = params.imageId;
	const match = Object.entries(manifest.albums || {})
		.flatMap(([, album]) => Object.entries(album.images || {}))
		.find(([key]) => key === imageId);
	if (!match) {
		error(404);
	}
	[, metadata] = match;
	const href = metadata.hrefs[HighResHref];
	src = `${StorageBaseUrl}/${href}.jpg`;
	titleStore.set(metadata.title);
	const [widthRatio, heightRatio] = metadata.aspectRatio.split(':').map((x) => Number(x));
	const width =
		widthRatio > heightRatio
			? HighResMaxDimension
			: (widthRatio / heightRatio) * HighResMaxDimension;
	const height =
		heightRatio > widthRatio
			? HighResMaxDimension
			: (heightRatio / widthRatio) * HighResMaxDimension;
	metaStore.set({
		'twitter:card': 'summary_large_image',
		'og:type': 'article',
		'og:image': src,
		'og:title': metadata.title,
		'og:description': 'Photography by Logan Dam',
		'og:image:width': width.toString(),
		'og:image:height': height.toString(),
		'og:image:alt': metadata.caption
	});
	date = new Date(metadata.captureDate);
	return {
		date,
		src,
		metadata
	};
};
