import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

export const load: PageLoad = async function ({ params, parent }) {
  const { manifest } = await parent();
  const imageId = params.imageId;
  const match = Object.entries(manifest.albums || {})
    .flatMap(([, album]) => Object.entries(album.images || {}))
    .find(([key]) => key === imageId);
  if (!match) {
    error(404);
  }
  const [, metadata] = match;

  return {
    metadata
  };
};
