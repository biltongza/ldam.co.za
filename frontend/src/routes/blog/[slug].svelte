<script context="module" lang="ts">
	import { title } from '$lib/title.store';
	import type { BlogResponse } from '$lib/types';
	import type { Load } from '@sveltejs/kit';
	import { onDestroy } from 'svelte';

	const load: Load = async function load({ params, fetch }) {
		const slug = params.slug;
		const response = await fetch(`${slug}.json`);
		if (!response.ok) {
			return {
				status: response.status,
				error: new Error(`Couldn't load blog post for slug '${slug}': ${response.status}`)
			};
		}
		const post: BlogResponse = await response.json();
		title.set(post.metadata.title);
		return {
			props: { post }
		};
	};

	export { load };
</script>

<script lang="ts">
	export let post: BlogResponse;
	onDestroy(() => {
		title.set(undefined);
	});
</script>

<h2>{post.metadata.title}</h2>
<p class="date">{post.metadata.date}</p>
{@html post.content}

<style lang="scss">
	.date {
		font-size: var(--sl-font-size-small);
	}
</style>
