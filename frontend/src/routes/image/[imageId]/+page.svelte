<script lang="ts">
	import { metaStore, titleStore } from '$lib/stores';
	import { onDestroy } from 'svelte';
	import type { PageData } from './$types';
	export let data: PageData;
	export let {src, metadata, date} = data;
	onDestroy(() => {
		titleStore.set(undefined);
		metaStore.set(undefined);
	});
</script>

<article class="image-container">
	<img {src} class="image" aria-labelledby="" alt={metadata.caption} />
	<section class="metadata">
		{#if metadata.title}
			<h2 class="title">{metadata.title}</h2>
		{/if}
		{#if metadata.caption}
			<h3 class="caption">{metadata.caption}</h3>
		{/if}
		<div class="date-info">
			<sl-icon name="calendar-event" />
			<div class="content">
				<time datetime={date.toISOString()}>{date.toLocaleDateString()}</time>
			</div>
		</div>
		<div class="camera-info">
			<sl-icon library="ionicons" name="camera-outline" />
			<div class="content">
				<div>{metadata.cameraModel}</div>
				<div>{metadata.lens}</div>
			</div>
		</div>
		<div class="exposure-info">
			<sl-icon library="ionicons" name="aperture-outline" />
			<div class="content">
				<span>ISO {metadata.iso}</span>
				<span>{metadata.focalLength}</span>
				<span>{metadata.fNumber}</span>
				<span>{metadata.shutterSpeed}</span>
			</div>
		</div>
		<div>
			This work is licensed under <a
				href="http://creativecommons.org/licenses/by-nc/4.0/?ref=chooser-v1"
				target="_blank"
				rel="license noopener noreferrer"
				style="display:inline-block;"
				>CC BY-NC 4.0<img
					style="height:22px!important;margin-left:3px;vertical-align:text-bottom;"
					src="https://mirrors.creativecommons.org/presskit/icons/cc.svg?ref=chooser-v1"
					alt="cc"
				/><img
					style="height:22px!important;margin-left:3px;vertical-align:text-bottom;"
					src="https://mirrors.creativecommons.org/presskit/icons/by.svg?ref=chooser-v1"
					alt="by"
				/><img
					style="height:22px!important;margin-left:3px;vertical-align:text-bottom;"
					src="https://mirrors.creativecommons.org/presskit/icons/nc.svg?ref=chooser-v1"
					alt="nc"
				/></a
			>
		</div>
	</section>
</article>

<style>
	.image {
		object-fit: contain;
		justify-content: center;
		background-image: url('/tail-spin.svg');
		background-repeat: no-repeat;
		background-position: center;
		height: 100%;
		width: 100%;
		max-height: 85vh;
	}

	.metadata {
		flex: 1 1 auto;
	}

	.image-container {
		display: flex;
		flex-direction: column;
		justify-content: center;
		gap: 2em;
	}
	@media only screen and (min-width: 768px) {
		.image-container {
			flex-direction: row;
			height: 90vh;
		}
	}

	.metadata {
		display: flex;
		flex-direction: column;
		gap: 1em;
	}

	.metadata > * > sl-icon {
		font-size: var(--sl-font-size-large);
	}

	.camera-info,
	.exposure-info,
	.date-info {
		display: flex;
		flex-direction: row;
		align-items: flex-start;
		justify-content: start;
		gap: 1em;
	}

	.exposure-info > .content {
		display: flex;
		flex-direction: row;
		justify-content: space-evenly;
		gap: 1em;
	}
</style>
