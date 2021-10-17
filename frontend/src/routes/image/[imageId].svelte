<script lang="ts">
	import { manifestStore } from '$lib/manifest-store';
	import { HighResHref } from '$lib/__consts';

	let src: string;
	export async function load({ page: { params } }) {
		const imageId = params.imageId;
		const manifest = await $manifestStore;
		const [_, image] = Object.entries(manifest.Albums)
			.flatMap(([_, album]) => Object.entries(album.Images))
			.find(([key]) => key === imageId);
		src = image.Hrefs[HighResHref];
		debugger;
	}
</script>

<!-- svelte-ignore a11y-missing-attribute -->
<img {src} />
