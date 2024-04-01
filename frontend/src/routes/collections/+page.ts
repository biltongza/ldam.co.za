import { metaStore, titleStore } from '$lib/stores';
import type { Album } from '$lib/types';
import type { PageLoad } from './$types';

let collections: Album[];

export const load: PageLoad = async function ({ parent }) {
  const { manifest } = await parent();
  collections = manifest.albums.filter((x) => !x.isPortfolio);

  metaStore.set({
    'twitter:card': 'summary',
    'og:type': 'article',
    'og:title': 'Collections',
    'og:description': 'Photography by Logan Dam'
  });

  titleStore.set('Collections');

  return {
    collections
  };
};
