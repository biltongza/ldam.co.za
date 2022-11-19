<script lang="ts">

	import AlbumPreview from '$lib/album/album-preview.svelte';
	import type { PageData } from './$types';
	export let data: PageData;
	export let {portfolio} = data;
</script>

<div>
	<div class="album-list">
		<AlbumPreview album={portfolio} />
	</div>
</div>
