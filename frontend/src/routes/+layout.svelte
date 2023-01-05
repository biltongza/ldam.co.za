<script context="module" lang="ts">
	import Footer from '$lib/footer/footer.svelte';
	import Header from '$lib/header/header.svelte';
	import { metaStore, titleStore } from '$lib/stores';
	import { defaultMetadata } from '$lib/__consts';
</script>

<script lang="ts">
	export let pageTitle: string;
	export let metas: { key: string; value: string }[];
	$: pageTitle = `${$titleStore ? $titleStore + ' - ' : ''}Logan Dam - Developer, Photographer`;
	$: metas = Object.entries({ ...defaultMetadata, ...$metaStore }).map(([key, value]) => ({
		key,
		value
	}));
</script>

<svelte:head>
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
		display: flex;
		flex: 1;
	}

	@media only screen and (min-width: 1224px) {
		.content {
			margin-left: var(--sl-spacing-2x-large);
			margin-right: var(--sl-spacing-2x-large);
		}
	}
</style>
