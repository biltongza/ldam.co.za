<script context="module" lang="ts">
	import AlbumPreview from '$lib/album/album-preview.svelte';
	import type { Album,Manifest } from '$lib/types';
	import type { Load } from '@sveltejs/kit';

	let portfolio: Album;
	export const load: Load = function ({ stuff }) {
		let manifest: Manifest = stuff.manifest;
		portfolio = manifest.albums.find(x => x.isPortfolio);

		return {};
	};
</script>

<div>
	<div class="album-list">
		<AlbumPreview album={portfolio} />
	</div>
</div>
