<script context="module" lang="ts">
	import AlbumPreview from '$lib/album-preview.svelte';
	import { manifestStore } from '$lib/manifest-store';
</script>

<div>
	{#await $manifestStore}
		Loading...
	{:then manifest}
		<div class="album-list">
			{#each Object.entries(manifest.Albums) as [album_key, album_value] (album_key)}
                <AlbumPreview album={album_value}></AlbumPreview>
            {/each}
		</div>
	{/await}
</div>
