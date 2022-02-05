import type { Manifest } from '$lib/types';
import { StorageBaseUrl } from '$lib/__consts';

export async function getManifest(): Promise<Manifest> {
	const response = await fetch(`${StorageBaseUrl}/manifest.json`);
	if (!response.ok) {
		throw new Error(`failed to load manifest: ${response.status}`);
	}
	const manifest = (await response.json()) as Manifest;

	return manifest;
}
