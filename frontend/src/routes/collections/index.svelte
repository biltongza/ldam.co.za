<script context="module" lang="ts">
	import AlbumPreview from '$lib/album/album-preview.svelte';
	import type { Album,Manifest } from '$lib/types';
	import type { Load } from '@sveltejs/kit';

	let collections: Album[];
	export const load: Load = function ({ stuff }) {
		let manifest: Manifest = stuff.manifest;
		collections = manifest.albums.filter((x) => !x.isPortfolio);

		return {};
	};
</script>

<div>
	<div class="collection-list">
		{#each collections as collection}
			<AlbumPreview album={collection} />
		{/each}
	</div>
</div>
