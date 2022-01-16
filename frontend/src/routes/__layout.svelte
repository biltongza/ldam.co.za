<script context="module" lang="ts">
	import Header from '$lib/header.svelte';
	import { title } from '$lib/title.store';
	import type { Manifest } from '$lib/types';
	import { StorageBaseUrl } from '$lib/__consts';
	import type { Load } from '@sveltejs/kit';
	let manifest: Manifest;
	const load: Load = async function ({ fetch }) {
		if (!manifest) {
			const response = await fetch(`${StorageBaseUrl}/manifest.json?t=${new Date().valueOf()}`);
			if (!response.ok) {
				return {
					status: response.status,
					error: new Error(`Couldn't load manifest: ${response.status}`)
				};
			}
			manifest = await response.json();
		}

		return {
			stuff: {
				manifest
			}
		};
	};

	export { load };
</script>

<script lang="ts">
	export let pageTitle: string;
	$: pageTitle = `${$title ? $title + ' - ' : ''}Logan Dam - Developer, Photographer`;
</script>

<svelte:head>
	<title>{pageTitle}</title>
</svelte:head>

<Header />
<div class="content">
	<slot />
</div>

<style lang="scss">
	.content {
		margin-left: var(--sl-spacing-small);
		margin-right: var(--sl-spacing-small);
		margin-bottom: var(--sl-spacing-large);
	}

	@media only screen and (min-width: 1224px) {
		.content {
			margin-left: var(--sl-spacing-2x-large);
			margin-right: var(--sl-spacing-2x-large);
		}
	}
</style>
