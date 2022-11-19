import { getBlogPosts } from '$lib/getBlogPosts';
import type { BlogMetadata } from '$lib/types';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = function () {
	
	const result: BlogMetadata[] = getBlogPosts();

	return {
		posts: result
	};
};
