<script lang="ts">
	import { metaStore, titleStore } from '$lib/stores';
	import { DateFormat } from '$lib/__consts';
	import dayjs from 'dayjs';
	import { onDestroy } from 'svelte';
	import type { PageData } from './$types';
	export let data: PageData;
	export let { post } = data;

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
	<p class="date">{dayjs(post.metadata.date).format(DateFormat)}</p>
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
