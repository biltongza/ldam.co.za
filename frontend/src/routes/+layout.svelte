<script lang="ts">
  import { page } from '$app/stores';
  import Footer from '$lib/footer/footer.svelte';
  import Header from '$lib/header/header.svelte';
  import { usePageMetadata, useTitle } from '$lib/stores.svelte';

  let { children } = $props();

  const title = useTitle();
  const metadata = usePageMetadata();

  let pageTitle = $derived(
    `${title.value ? title.value + ' - ' : ''}Logan Dam - Software Engineer, Photographer`
  );

  let defaultMetadata = $derived({
    'og:title': 'Logan Dam - Software Engineer, Photographer',
    'og:type': 'website',
    'og:description': "Logan Dam's portfolio of photography and code.",
    'og:image': 'https://ldam.co.za/favicon-310.png',
    'og:locale': 'en_ZA',
    'og:url': $page.url.toString()
  });

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
