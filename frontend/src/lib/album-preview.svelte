<script lang="ts">
	import type { Album } from './manifest';
	import Thumbnail from './thumbnail.svelte';
	export let album: Album;
</script>

<div class="album-container">
	<h3>{album.Title}</h3>
	<div class="thumbnail-container">
		{#each Object.entries(album.Images) as [image_key, image_value] (image_key)}
			<Thumbnail image={image_value} />
		{/each}
	</div>
</div>

<style>
	.thumbnail-container {
		display: grid;
		max-width: 1200px;
		margin-left: auto;
		margin-right: auto;
		column-gap: var(--sl-spacing-xx-large);
		row-gap: var(--sl-spacing-large);
		place-items: center;
	}

	@media only screen and (min-width: 768px) {
		.thumbnail-container {
			grid-template-columns: repeat(3, 1fr);
		}
	}
</style>
