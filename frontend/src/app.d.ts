/* eslint-disable @typescript-eslint/no-empty-interface */
/// <reference types="@sveltejs/kit" />
type Typify<T> = { [K in keyof T]: Typify<T[K]> };
declare namespace App {
	import type { Manifest } from '$lib/types';

	interface Stuff {
		manifest: Manifest;
	}
}
