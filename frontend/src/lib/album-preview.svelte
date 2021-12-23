<script lang="ts">
	import type { Album } from './manifest';
	import Thumbnail from './thumbnail.svelte';
	export let album: Album;
	export let numberOfImages: number = Number.MAX_SAFE_INTEGER;

	const entries = Object.entries(album.images).map(([, meta]) => meta);
	const images = entries
		.sort((meta1, meta2) => {
			const a = new Date(meta1.captureDate).valueOf();
			const b = new Date(meta2.captureDate).valueOf();

			return Number(b > a) - Number(b < a);
		})
		.slice(0, numberOfImages);
</script>

<div class="album-container">
	<h3>{album.title}</h3>
	<div class="thumbnail-container">
		{#each images as image (image.id)}
			<Thumbnail {image} />
		{/each}
	</div>
</div>

<style>
	.thumbnail-container {
		display: grid;
		column-gap: var(--sl-spacing-2x-large);
		row-gap: var(--sl-spacing-large);
		place-items: center;
		justify-content: center;
	}

	@media only screen and (min-width: 768px) {
		.thumbnail-container {
			grid-auto-rows: 320px;
			grid-template-columns: repeat(2, fit-content(320px));
		}
	}

	@media only screen and (min-width: 1030px) {
		.thumbnail-container {
			grid-template-columns: repeat(3, fit-content(320px));
		}
	}
</style>
