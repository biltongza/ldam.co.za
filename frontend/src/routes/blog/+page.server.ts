import { getBlogPosts } from '$lib/getBlogPosts';
import type { BlogMetadata } from '$lib/types';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async function ({ platform }) {
  const result: BlogMetadata[] = await getBlogPosts(platform.context.log.info);

  return {
    posts: result
  };
};
