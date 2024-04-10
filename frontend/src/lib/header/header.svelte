<script lang="ts">
  import { website } from '$lib/info';
  import Navigation from './navigation.svelte';
</script>

<header class="header">
  <a class="brand h-card" href={website}><h1>Logan Dam</h1></a>
  <Navigation />
</header>

<style lang="scss">
  :root {
    --header-height: 60px;
  }

  h1 {
    text-transform: uppercase;
    font-size: 100%;
  }

  .header {
    padding-left: var(--sl-spacing-small);
    padding-right: var(--sl-spacing-small);
    background-color: var(--sl-color-neutral-100);
    margin-bottom: var(--sl-spacing-small);
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    //align-items: baseline;

    @media only screen and (min-width: 1224px) {
      padding-left: var(--sl-spacing-2x-large);
      padding-right: var(--sl-spacing-2x-large);
    }
  }

  .brand {
    font-size: var(--sl-font-size-x-large);
  }
</style>
