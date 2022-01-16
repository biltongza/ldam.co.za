import { process } from '$lib/markdown';

export async function get({ params }) {
	// we could get the dynamic slug from the parameter of get.
	const { slug } = params;

	const { metadata, content } = process(`src/posts/${slug}.md`);
	const body = JSON.stringify({ metadata, content });

	return { body };
}
