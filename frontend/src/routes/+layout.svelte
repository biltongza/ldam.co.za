<script context="module" lang="ts">
  import { defaultMetadata } from '$lib/__consts';
  import Footer from '$lib/footer/footer.svelte';
  import Header from '$lib/header/header.svelte';
  import { metaStore, titleStore } from '$lib/stores';
  import 'unfonts.css';
  import { links } from 'unplugin-fonts/head';
</script>

<script lang="ts">
  export let pageTitle: string;
  export let metas: { key: string; value: string }[];
  $: pageTitle = `${$titleStore ? $titleStore + ' - ' : ''}Logan Dam - Software Engineer, Photographer`;
  $: metas = Object.entries({ ...defaultMetadata, ...$metaStore }).map(([key, value]) => ({
    key,
    value
  }));
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
  <slot />
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
