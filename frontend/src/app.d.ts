/* eslint-disable @typescript-eslint/no-empty-interface */
/// <reference types="@sveltejs/kit" />

declare namespace App {
	import type { Manifest } from '$lib/types';

	interface Stuff {
		manifest: Manifest;
	}
}
