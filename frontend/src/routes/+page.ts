import type { Album } from '$lib/types';
import type { PageLoad } from './$types';

let portfolio: Album;

export const load: PageLoad = async function ({ parent }) {
  const { manifest } = await parent();
  portfolio = manifest.albums.find((x) => x.isPortfolio);

  return {
    portfolio
  };
};
