import { getBlogPosts } from '$lib/getBlogPosts';
import type { BlogMetadata } from '$lib/types';
import type { RequestHandler } from '@sveltejs/kit';

export const GET: RequestHandler = function () {
	
	const result: Typify<BlogMetadata[]> = getBlogPosts();

	return {
		body: {
			posts: result
		},
	};
};
