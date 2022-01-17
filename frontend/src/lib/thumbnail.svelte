<script context="module" lang="ts">
	const thumbnailSizes = {
		thumbnail2x: '320w',
		'640': '640w',
		'1280': '1280w',
	};
</script>

<script lang="ts">
	import type { ImageMetadata } from './types';
	import {
	StorageBaseUrl,
	ThumbnailHrefNormalDensity
	} from './__consts';

	export let image: ImageMetadata = undefined;
	let imageRoute: string;
	let src: string;
	let srcSet: string;

	$: {
		imageRoute = `/image/${image.id}`;
		src = `${StorageBaseUrl}/${image.hrefs[ThumbnailHrefNormalDensity]}`;
		srcSet = Object.entries(thumbnailSizes).map(([key, value]) => `${StorageBaseUrl}/${image.hrefs[key]} ${value}`).join(', ');
	}
</script>

<a href={imageRoute}>
	<img {src} class="thumbnail-image" loading="lazy" alt="" {srcSet}/>
</a>

<style>
	a {
		display: block;
		width: 100%;
		height: 100%;
	}
	.thumbnail-image {
		width: 100%;
		height: 100%;
		object-fit: contain;
		max-height: 85vh;
		background-image: url('/tail-spin.svg');
		background-repeat: no-repeat;
		background-position: center;
	}
</style>
