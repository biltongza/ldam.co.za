<script context="module" lang="ts">
	import type { Manifest } from '$lib/manifest';
	import { HighResHref,StorageBaseUrl } from '$lib/__consts';
	import type { Load } from '@sveltejs/kit';
	
	let src: string;
	const load: Load = function({ page: { params }, stuff }) {
		const manifest: Manifest = stuff.manifest;
		const imageId = params.imageId;
		const [_, image] = Object.entries(manifest.Albums)
			.flatMap(([_, album]) => Object.entries(album.Images))
			.find(([key]) => key === imageId);
		const href = image.Hrefs[HighResHref]; 
		src = `${StorageBaseUrl}/${href}`;

		return {};
	};

	export { load };
</script>

<!-- svelte-ignore a11y-missing-attribute -->
<img {src} />
