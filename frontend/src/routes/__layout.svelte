<script context="module" lang="ts">
	import Header from '$lib/header.svelte';
	import type { Manifest } from '$lib/manifest';
	import { StorageBaseUrl } from '$lib/__consts';
	import type { Load } from '@sveltejs/kit';
	let manifest: Manifest;
	const load: Load = async function ({ fetch }) {
		if (!manifest) {
			const response = await fetch(`${StorageBaseUrl}/manifest.json`);
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

<Header />
<slot />
