import { process } from '$lib/blog/markdown';
import type { BlogResponse } from '$lib/types';
import type { RequestHandler } from '@sveltejs/kit';

export const GET: RequestHandler = async function ({ params }) {
	// we could get the dynamic slug from the parameter of get.
	const { slug } = params;

	const blogResponse: BlogResponse = process(`src/posts/${slug}.md`);
	const result: Typify<BlogResponse> = blogResponse;

	return {
		body: {
			post: result
		}
	};
};
