/// <reference types="@sveltejs/kit" />
/// <reference types="svelte-adapter-azure-swa" />
import type { Manifest } from '$lib/types';

type Typify<T> = { [K in keyof T]: Typify<T[K]> };
declare namespace App {
  interface Error {
    message: string;
  }

  interface Stuff {
    manifest: Manifest;
  }
}

// App version
declare const __VERSION__: string;
// Date of last commit
declare const __LASTMOD__: string;
