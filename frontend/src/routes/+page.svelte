<script lang="ts">
  import AlbumPreview from '$lib/album/album-preview.svelte';

  let { data } = $props();
</script>

<div>
  <div class="album-list">
    <AlbumPreview album={data.portfolio} showHeader={false} />
  </div>
</div>
