<script context="module" lang="ts">
	import { base } from '$app/paths';
	import type { BlogMetadata } from '$lib/types';
	import type { Load } from '@sveltejs/kit';
	import dayjs from 'dayjs';
	const load: Load = async function load({ fetch }) {
		const response = await fetch(`${base}/blog/posts.json`);
		if (!response.ok) {
			return {
				status: response.status,
				error: new Error(`Couldn't load blog index: ${response.status}`)
			};
		}
		const posts: BlogMetadata[] = await response.json();
		const sortedPosts = posts.sort((a, b) => dayjs(b.date).valueOf() - dayjs(a.date).valueOf());
		return {
			props: { posts: sortedPosts }
		};
	};
	export { load };
</script>

<script lang="ts">
	export let posts: BlogMetadata[] = [];
</script>

<div>
	<p class="info">{posts.length} posts.</p>
	{#each posts as post}
		<div class="post">
			<a href={`${base}/blog/${post.slug}`}>
				<h2 class="title">{post.title}</h2>
			</a>
			<p class="date">{post.date}</p>
			<p>
				{#each post.tags as tag}
					<sl-badge variant="primary" pill>{tag}</sl-badge>
				{/each}
			</p>
			<p>{post.excerpt}</p>
		</div>
	{/each}
</div>

<style lang="scss">
	.date {
		font-size: var(--sl-font-size-small);
	}
</style>
