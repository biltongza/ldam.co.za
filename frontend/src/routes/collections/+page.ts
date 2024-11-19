import type { Album } from '$lib/types';
import type { PageLoad } from './$types';

let collections: Album[];

export const load: PageLoad = async function ({ parent }) {
  const { manifest } = await parent();
  collections = manifest.albums.filter((x) => !x.isPortfolio);

  return {
    collections
  };
};
