import { writable } from 'svelte/store';
import { defaultMetadata } from './__consts';

let title: string = $state<string | undefined>(undefined);
let meta: Record<string, string> = $state(defaultMetadata);

export function useTitle() {
  return {
    get value() {
      return title;
    },

    set value(newValue) {
      title = newValue;
    }
  };
}

export function usePageMetadata() {
  return {
    get value() {
      return meta;
    },

    clear() {
      meta = defaultMetadata;
    },

    push(metas: Record<string, string>) {
      meta = {
        ...meta,
        ...metas
      };
    }
  };
}

export const metaStore = writable<{ [key: string]: string }>(defaultMetadata);
