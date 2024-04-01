import { getBlogPosts } from '$lib/getBlogPosts';
import type { BlogMetadata } from '$lib/types';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async function () {
  const result: BlogMetadata[] = await getBlogPosts();

  return {
    posts: result
  };
};
