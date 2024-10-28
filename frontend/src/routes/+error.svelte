<script module lang="ts">
	import { page } from '$app/stores';
</script>

<h3>{$page.status}</h3>
{#if $page.status === 404}
	<p>That page doesn't seem to exist :(</p>
{:else}
	<p>Something went wrong :(</p>
{/if}
