import type { Asyncable } from 'svelte-asyncable';
import { asyncable } from 'svelte-asyncable';
import type { Manifest } from './manifest';
import { StorageBaseUrl } from './__consts';

export const manifestStore: Asyncable<Manifest> = asyncable(async () => {
	const response = await fetch(`${StorageBaseUrl}/manifest.json`);
	if (!response.ok) {
		throw new Error(`Couldn't load manifest: ${response.status}`);
	}
	return response.json();
});
