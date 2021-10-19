<script lang="ts">
	import type { Album } from './manifest';
	import Thumbnail from './thumbnail.svelte';
	export let album: Album;
	export let numberOfImages: number = Number.MAX_SAFE_INTEGER;

	const entries = Object.entries(album.Images).map(([, meta]) => meta);
	const images = entries
		.sort((meta1, meta2) => {
			const a = new Date(meta1.CaptureDate).valueOf();
			const b = new Date(meta2.CaptureDate).valueOf();

			return Number(b > a) - Number(b < a);
		})
		.slice(0, numberOfImages);
</script>

<div class="album-container">
	<h3>{album.Title}</h3>
	<div class="thumbnail-container">
		{#each images as image (image.Id)}
			<Thumbnail {image} />
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
