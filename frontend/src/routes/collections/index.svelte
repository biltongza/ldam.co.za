<script context="module" lang="ts">
    import AlbumPreview from '$lib/album/album-preview.svelte';
    import { metaStore,titleStore } from '$lib/stores';
    import type { Album,Manifest } from '$lib/types';
    import type { Load } from '@sveltejs/kit';

	let collections: Album[];
	export const load: Load = function ({ stuff }) {
		let manifest: Manifest = stuff.manifest;
		collections = manifest.albums.filter((x) => !x.isPortfolio);

        metaStore.set({
			'twitter:card': 'summary',
			'og:type': 'article',
			'og:title': 'Collections',
			'og:description': 'Photography by Logan Dam',
		});

        titleStore.set('Collections');

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
