<script>
	/**
	 * @typedef {Object} Props
	 * @property {import('svelte').Snippet} [children]
	 */

	/** @type {Props} */
	let { children } = $props();
</script>

<section class="blog">
	{@render children?.()}
</section>

<style>
	.blog {
		max-width: 900px;
		margin-left: auto;
		margin-right: auto;
	}
</style>
