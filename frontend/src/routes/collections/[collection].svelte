<script context="module" lang="ts">
	import AlbumPreview from '$lib/album/album-preview.svelte';
	import type { Album,Manifest } from '$lib/types';
	import type { Load } from '@sveltejs/kit';

	let collection: Album;
	export const load: Load = function ({ stuff, params }) {
		let manifest: Manifest = stuff.manifest;
        const slug = params['collection'];
		collection = manifest.albums.find(x => x.id === slug);

		return {};
	};
</script>

<AlbumPreview album={collection} />
