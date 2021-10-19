<script context="module" lang="ts">
	import type { IApplicationInsights } from '@microsoft/applicationinsights-web';
	import type { ErrorLoad } from '@sveltejs/kit';

	const appInsights: IApplicationInsights = (<any>window).appInsights;
	const load: ErrorLoad = function ({ error, status, page }) {
		appInsights.trackException({
			error: error,
            properties: {
                status,
                ...page,
            },
		});
		return {
            props: {
                status,
            }
        };
	};

	export { load };
</script>
<script lang="ts">
    export let status: number;
</script>

<h3>{status}</h3>
<p>Something went wrong :(</p>
