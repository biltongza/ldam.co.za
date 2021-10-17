<script lang="ts">
	import type { ImageMetadata } from './manifest';
	import { MaxDimensionHighDensity,MaxDimensionNormalDensity,StorageBaseUrl,ThumbnailHrefHighDensity,ThumbnailHrefNormalDensity } from './__consts';

	export let image: ImageMetadata = undefined;
	let imageRoute: string;
	let src: string;
	let width: number;
	let height: number;

	$: {
		imageRoute = `/image/${image.Id}`;
		const maxDimension =
			window.devicePixelRatio > 1 ? MaxDimensionHighDensity : MaxDimensionNormalDensity;
		const ratio = Math.min(maxDimension / image.Width, maxDimension / image.Height);
		width = image.Width * ratio;
		height = image.Height * ratio;
        const thumbnailHrefKey = window.devicePixelRatio > 1 ? ThumbnailHrefHighDensity : ThumbnailHrefNormalDensity;
        src = `${StorageBaseUrl}/${image.Hrefs[thumbnailHrefKey]}`;
	}
</script>

<a href={imageRoute}>
	<!-- svelte-ignore a11y-missing-attribute -->
	<img
		{src}
		class="thumbnail"
		loading="lazy"
		style="max-width: {width}; max-height: {height}"
		{width}
		{height}
	/>
</a>
