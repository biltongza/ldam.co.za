<script context="module" lang="ts">
	const thumbnailSizes = {
		thumbnail2x: '320w',
		'640': '640w',
		'1280': '1280w'
	};
	const formats = {
		jpg: 'image/jpeg',
		webp: 'image/webp'
	};
</script>

<script lang="ts">
	import type { ImageMetadata } from '$lib/types';
	import { StorageBaseUrl,ThumbnailHrefNormalDensity } from '$lib/__consts';

	export let image: ImageMetadata = undefined;
	let imageRoute: string;
	let src: string;
	let srcSets: { srcSet: string; mime: string }[];

	$: {
		imageRoute = `/image/${image.id}`;
		src = `${StorageBaseUrl}/${image.hrefs[ThumbnailHrefNormalDensity]}.webp`;
		srcSets = Object.entries(thumbnailSizes).flatMap(([sizeKey, maxWidth]) =>
			Object.entries(formats).map(([extension, mime]) => ({
				srcSet: `${StorageBaseUrl}/${image.hrefs[sizeKey]}.${extension} ${maxWidth}`,
				mime
			}))
		);
	}
</script>

<a href={imageRoute}>
	<picture>
		{#each srcSets as srcset}
			<source srcset={srcset.srcSet} type={srcset.mime} />
		{/each}
		<img {src} class="thumbnail-image" loading="lazy" alt="" />
	</picture>
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
