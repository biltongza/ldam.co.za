import { getManifest } from '$lib/getManifest';
import type { Manifest } from '$lib/types';
import type { LayoutLoad } from './$types';

let manifest: Manifest;
export const load: LayoutLoad = async function () {
	if (!manifest) {
		manifest = await getManifest();
	}

	return {
		manifest
	};
};
