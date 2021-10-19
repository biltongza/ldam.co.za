<script context="module" lang="ts">
	import type { ImageMetadata,Manifest } from '$lib/manifest';
	import { HighResHref,StorageBaseUrl } from '$lib/__consts';
	import type { Load } from '@sveltejs/kit';
	
	let src: string;
	let metadata: ImageMetadata;
	const load: Load = function({ page: { params }, stuff }) {
		const manifest: Manifest = stuff.manifest;
		const imageId = params.imageId;
		[, metadata] = Object.entries(manifest.Albums)
			.flatMap(([_, album]) => Object.entries(album.Images))
			.find(([key]) => key === imageId);
		const href = metadata.Hrefs[HighResHref]; 
		src = `${StorageBaseUrl}/${href}`;
		return {};
	};

	export { load };
</script>

<!-- svelte-ignore a11y-missing-attribute -->
<img {src} />

<style>
	img {
		width: 100%;
	}
</style>