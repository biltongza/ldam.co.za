<script context="module" lang="ts">
	import AlbumPreview from '$lib/album-preview.svelte';
	import type { Manifest } from '$lib/manifest';
	import type { Load } from '@sveltejs/kit';

	let manifest: Manifest;
	const load: Load = function ({ stuff }) {
		manifest = stuff.manifest;

		return {};
	};

	export { load };
</script>

<div>
	<div class="album-list">
		{#each Object.entries(manifest.albums) as [album_key, album_value] (album_key)}
			<AlbumPreview album={album_value} />
		{/each}
	</div>
</div>
