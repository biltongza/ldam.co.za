<script module lang="ts">
  import { defaultMetadata } from '$lib/__consts';
  import Footer from '$lib/footer/footer.svelte';
  import Header from '$lib/header/header.svelte';
  import { usePageMetadata, useTitle } from '$lib/stores.svelte';
</script>

<script lang="ts">
  let { children } = $props();
  const title = useTitle();
  const metadata = usePageMetadata();
  let pageTitle = $derived(
    `${title.value ? title.value + ' - ' : ''}Logan Dam - Software Engineer, Photographer`
  );

  let metas = $derived(
    Object.entries({ ...defaultMetadata, ...metadata.value }).map(([key, value]) => ({
      key,
      value
    }))
  );
</script>

<svelte:head>
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
