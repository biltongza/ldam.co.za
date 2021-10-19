<script context="module" lang="ts">
	import Header from '$lib/header.svelte';
	import type { Manifest } from '$lib/manifest';
	import { StorageBaseUrl } from '$lib/__consts';
	import type { Load } from '@sveltejs/kit';
	const load: Load = async function ({ fetch }) {
		const response = await fetch(`${StorageBaseUrl}/manifest.json`);
		if (!response.ok) {
			return {
				status: response.status,
				error: new Error(`Couldn't load manifest: ${response.status}`),
			};
		}
		const manifest: Manifest = await response.json();

		return {
			stuff: {
				manifest
			}
		};
	};
	export { load };
</script>

<Header />
<slot />
