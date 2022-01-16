import { writable } from 'svelte/store';
import { defaultMetadata } from './__consts';

export const title = writable<string>();
export const meta = writable<{ [key: string]: string }>(defaultMetadata);
