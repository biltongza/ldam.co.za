<script lang="ts">
	import { base } from '$app/paths';
	import { DateFormat } from '$lib/__consts';
	import dayjs from 'dayjs';
	import type { PageData } from './$types';
	export let data: PageData;

	export let {posts} = data;
</script>

<div>
	<p class="info">{posts.length} posts.</p>
	{#each posts as post}
		<div class="post">
			<a href={`${base}/blog/${post.slug}`}>
				<h2 class="title">{post.title}</h2>
			</a>
			<p class="date">{dayjs(post.date).format(DateFormat)}</p>
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
