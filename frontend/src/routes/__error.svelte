<script context="module" lang="ts">
	import { browser } from '$app/env';
	import type { IApplicationInsights } from '@microsoft/applicationinsights-web';
	import type { ErrorLoad } from '@sveltejs/kit';

	const appInsights: IApplicationInsights = browser && (<any>window).appInsights;
	export const load: ErrorLoad = function ({ error, status, url, params }) {
		appInsights &&
			appInsights.trackException({
				error: error,
				properties: {
					status,
					url,
					...params
				}
			});
		return {
			props: {
				status
			}
		};
	};
</script>

<script lang="ts">
	export let status: number;
</script>

<h3>{status}</h3>
{#if status === 404}
	<p>That page doesn't seem to exist :(</p>
{:else}
	<p>Something went wrong :(</p>
{/if}
