<script lang="ts">
  import AlbumPreview from '$lib/album/album-preview.svelte';
  let { data } = $props();
</script>

<div>
  <div class="collection-list">
    {#each data.collections as collection}
      <AlbumPreview album={collection} />
    {/each}
  </div>
</div>
