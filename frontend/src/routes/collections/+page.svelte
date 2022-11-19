<script lang="ts">

    import AlbumPreview from '$lib/album/album-preview.svelte';
    import type { PageData } from './$types';
	export let data: PageData;
	export let {collections} = data;
</script>

<div>
	<div class="collection-list">
		{#each collections as collection}
			<AlbumPreview album={collection} />
		{/each}
	</div>
</div>
