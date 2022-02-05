import { writable } from 'svelte/store';
import { defaultMetadata } from './__consts';

export const titleStore = writable<string>();
export const metaStore = writable<{ [key: string]: string }>(defaultMetadata);
