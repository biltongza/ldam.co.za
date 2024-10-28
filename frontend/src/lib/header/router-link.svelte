<script lang="ts">
  import { run } from 'svelte/legacy';

  import { page } from '$app/stores';
  interface Props {
    path: string;
    children?: import('svelte').Snippet;
  }

  let { path, children }: Props = $props();
  let matches = $state(false);
  run(() => {
    const routeSegments = path?.split('/') || [];
    const activeRouteSegments = $page.url.pathname?.split('/') || [];
    matches = routeSegments.every((segment, index) => activeRouteSegments[index] === segment);
  });
</script>

<a href={path} class:active={matches}>{@render children?.()}</a>

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
