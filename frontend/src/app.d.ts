/* eslint-disable @typescript-eslint/no-empty-interface */
/// <reference types="@sveltejs/kit" />

import type { Manifest } from '$lib/types';

type Typify<T> = { [K in keyof T]: Typify<T[K]> };
declare namespace App {
	interface Error {
		message: string,
	}

	interface Stuff {
		manifest: Manifest;
	}
}
