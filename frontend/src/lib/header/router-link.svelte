<script lang="ts">
	import { page } from '$app/stores';
	export let path: string;
	let matches = false;
	$: {
		const routeSegments = path?.split('/') || [];
		const activeRouteSegments = $page.url.pathname?.split('/') || [];
		matches = routeSegments.every((segment, index) => activeRouteSegments[index] === segment);
	}
</script>

<a href={path} class:active={matches}><slot /></a>

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
