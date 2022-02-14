<script lang="ts">
	import { metaStore,titleStore } from '$lib/stores';
	import type { BlogResponse } from '$lib/types';
	import { onDestroy } from 'svelte';
	export let post: BlogResponse;

	titleStore.set(post.metadata.title);
	metaStore.set({
		'og:type': 'article',
		'og:title': post.metadata.title,
		'og:description': post.metadata.excerpt
	});

	onDestroy(() => {
		titleStore.set(undefined);
		metaStore.set(undefined);
	});
</script>

<article>
	<h2>{post.metadata.title}</h2>
	<p class="date">{post.metadata.date}</p>
	<p>
		{#each post.metadata.tags as tag}
			<sl-badge variant="primary" pill>{tag}</sl-badge>
		{/each}
	</p>
	{@html post.content}
</article>

<style lang="scss">
	.date {
		font-size: var(--sl-font-size-small);
	}
</style>
