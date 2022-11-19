import { process } from '$lib/blog/markdown';
import type { BlogResponse } from '$lib/types';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async function ({ params }) {
	// we could get the dynamic slug from the parameter of get.
	const { slug } = params;

	const post: BlogResponse = process(`src/posts/${slug}.md`);

	return {
		post
	};
};
