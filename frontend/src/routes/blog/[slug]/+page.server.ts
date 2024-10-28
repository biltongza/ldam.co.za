import { process } from '$lib/blog/markdown';
import { getBlogPosts } from '$lib/getBlogPosts';
import type { BlogResponse } from '$lib/types';
import type { EntryGenerator, PageServerLoad } from './$types';

export const load: PageServerLoad = async function ({ params }) {
  // we could get the dynamic slug from the parameter of get.
  const { slug } = params;

  const post: BlogResponse = await process(`src/posts/${slug}.md`);

  return {
    post
  };
};

export const entries: EntryGenerator = async () => {
  const blogs = await getBlogPosts();
  return blogs.map((b) => ({ slug: b.slug }));
};
