<script lang="ts">
  import { resolve } from '$app/paths';
  import { page } from '$app/state';
  import type { Pathname } from '$app/types';
  interface Props {
    path: Pathname;
    children?: import('svelte').Snippet;
  }

  let { path, children }: Props = $props();
  let routeSegments = $derived(path?.split('/') || []);
  let activeRouteSegments = $derived(page.url.pathname?.split('/') || []);
  let matches = $derived(
    routeSegments.every((segment, index) => activeRouteSegments[index] === segment)
  );
</script>

<a href={resolve(path)} class:active={matches}>{@render children?.()}</a>

<style>
  a.active {
    border-bottom: 1px solid black;
  }
  a.active:hover {
    border-bottom-color: var(--sl-color-primary-600);
  }
  a:hover {
    color: var(--sl-color-primary-600);
  }
</style>
