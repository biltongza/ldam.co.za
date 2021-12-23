<script context="module" lang="ts">
	import type { ImageMetadata,Manifest } from '$lib/manifest';
	import { HighResHref,StorageBaseUrl } from '$lib/__consts';
	import type { Load } from '@sveltejs/kit';

	let src: string;
	let metadata: ImageMetadata;
	const load: Load = function ({ page: { params }, stuff }) {
		const manifest: Manifest = stuff.manifest;
		const imageId = params.imageId;
		[, metadata] = Object.entries(manifest.albums)
			.flatMap(([_, album]) => Object.entries(album.images))
			.find(([key]) => key === imageId);
		const href = metadata.hrefs[HighResHref];
		src = `${StorageBaseUrl}/${href}`;
		return {};
	};

	export { load };
</script>

<div class="image-container">
	<img class="image" {src} alt={metadata.caption} />
	<div class="metadata">
		{#if metadata.title}
			<h3>{metadata.title}</h3>
		{/if}
		{#if metadata.caption}
			<h4>{metadata.caption}</h4>
		{/if}
		<div class="date-info">
			<sl-icon name="calendar-event"></sl-icon>
			<div class="content">
				<span>{new Date(metadata.captureDate).toLocaleDateString()}</span>
			</div>
		</div>
		<div class="camera-info">
			<sl-icon library="ionicons" name="camera-outline"></sl-icon>
			<div class="content">
				<div>{metadata.cameraModel}</div>
				<div>{metadata.lens}</div>
			</div>
		</div>
		<div class="exposure-info">
			<sl-icon library="ionicons" name="aperture-outline"></sl-icon>
			<div class="content">
				<span>ISO {metadata.iso}</span>
				<span>{metadata.focalLength}</span>
				<span>{metadata.fNumber}</span>
				<span>{metadata.shutterSpeed}</span>
			</div>
		</div>
	</div>
</div>

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
