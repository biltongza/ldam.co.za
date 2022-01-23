import { process } from '$lib/blog/markdown';
import type { BlogResponse } from '$lib/types';

export async function get({ params }) {
	// we could get the dynamic slug from the parameter of get.
	const { slug } = params;

	const blogResponse: BlogResponse = process(`src/posts/${slug}.md`);
	const body = JSON.stringify(blogResponse);

	return { body };
}
