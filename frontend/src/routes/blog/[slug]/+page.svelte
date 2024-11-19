<script lang="ts">
  import { DateFormat } from '$lib/__consts';
  import { usePageMetadata, useTitle } from '$lib/stores.svelte.js';
  import dayjs from 'dayjs';
  import { onMount } from 'svelte';
  let { data } = $props();
  let { post } = data;
  const title = useTitle();
  const metadata = usePageMetadata();

  onMount(() => {
    title.value = post.metadata.title;

    metadata.push({
      'og:type': 'article',
      'og:title': post.metadata.title,
      'og:description': post.metadata.excerpt
    });

    return () => {
      title.value = undefined;
      metadata.clear();
    };
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
