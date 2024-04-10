<script lang="ts">
  import { DateFormat } from '$lib/__consts';
  import { metaStore, titleStore } from '$lib/stores';
  import dayjs from 'dayjs';
  import { onDestroy } from 'svelte';
  import type { PageData } from './$types';
  export let data: PageData;
  export let { post } = data;

  titleStore.set(post.metadata.title);
  metaStore.set({
    'og:type': 'article',
    'og:title': post.metadata.title,
    'og:description': post.metadata.excerpt
  });

  onDestroy(() => {
    titleStore.set(undefined);
    metaStore.set(undefined);
  });
</script>

<article class="h-entry">
  <h2 class="p-name">{post.metadata.title}</h2>
  <p></p>
  <time class="date dt-published" datetime={dayjs(post.metadata.date).format()}
    >{dayjs(post.metadata.date).format(DateFormat)}</time
  >
  <p>
    {#each post.metadata.tags as tag}
      <sl-badge variant="primary" class="p-category" pill>{tag}</sl-badge>
    {/each}
  </p>
  <div class="e-content">
    {@html post.content}
  </div>
</article>

<style lang="scss">
  .date {
    font-size: var(--sl-font-size-small);
  }
</style>
