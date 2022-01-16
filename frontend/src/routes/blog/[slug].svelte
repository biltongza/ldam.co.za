<script context="module" lang="ts">
	import type { Load } from '@sveltejs/kit';

	const load: Load = async function load({ params, fetch }) {
		const slug = params.slug;
		const response = await fetch(`${slug}.json`);
		if(!response.ok) {
			return {
				status: response.status,
				error: new Error(`Couldn't load blog post for slug '${slug}': ${response.status}`)
			};
		}
		const post = await response.json();
		return {
			props: { post }
		};
	};
	export { load };
</script>

<script>
	// post will have metadata and content
	export let post;
</script>

{@html post.content}
