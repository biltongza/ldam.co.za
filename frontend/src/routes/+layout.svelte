<script module lang="ts">
  import { defaultMetadata } from '$lib/__consts';
  import Footer from '$lib/footer/footer.svelte';
  import Header from '$lib/header/header.svelte';
  import { metaStore, titleStore } from '$lib/stores';
  import 'unfonts.css';
  import { links } from 'unplugin-fonts/head';
</script>

<script lang="ts">
  import { run } from 'svelte/legacy';

  interface Props {
    pageTitle: string;
    metas: { key: string; value: string }[];
    children?: import('svelte').Snippet;
  }

  let { pageTitle = $bindable(), metas = $bindable(), children }: Props = $props();
  run(() => {
    pageTitle = `${$titleStore ? $titleStore + ' - ' : ''}Logan Dam - Software Engineer, Photographer`;
  });
  run(() => {
    metas = Object.entries({ ...defaultMetadata, ...$metaStore }).map(([key, value]) => ({
      key,
      value
    }));
  });
</script>

<svelte:head>
  {#each links as link}
    <link {...link?.attrs || {}} />
  {/each}
  <title>{pageTitle}</title>
  {#each metas as m (m.key)}
    <meta property={m.key} content={m.value} />
  {/each}
</svelte:head>

<Header />
<main class="content">
  {@render children?.()}
</main>
<Footer />

<style lang="scss">
  .content {
    margin-left: var(--sl-spacing-small);
    margin-right: var(--sl-spacing-small);
    margin-bottom: var(--sl-spacing-large);
    flex: 1;
  }

  @media only screen and (min-width: 1224px) {
    .content {
      margin-left: var(--sl-spacing-2x-large);
      margin-right: var(--sl-spacing-2x-large);
    }
  }
</style>
