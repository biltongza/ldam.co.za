<script lang="ts">
  import AlbumPreview from '$lib/album/album-preview.svelte';
  import { usePageMetadata, useTitle } from '$lib/stores.svelte';
  import { onMount } from 'svelte';

  let { data } = $props();
  const title = useTitle();
  const metadata = usePageMetadata();

  onMount(() => {
    title.value = 'Collections';
    metadata.set({
      'twitter:card': 'summary',
      'og:type': 'article',
      'og:title': 'Collections',
      'og:description': 'Photography by Logan Dam'
    });
    return () => {
      title.value = undefined;
      metadata.clear();
    };
  });
</script>

<div>
  <div class="collection-list">
    {#each data.collections as collection}
      <AlbumPreview album={collection} />
    {/each}
  </div>
</div>
