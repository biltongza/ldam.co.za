import { rehypeConverter, remarkParser } from '$lib/blog/markdown';
import path from 'path';
import { read } from 'to-vfile';
import type { PageServerLoad } from './$types';

export const prerender = true;

export const load: PageServerLoad = async () => {
  const content = await read(path.resolve('src/routes/resume/resume.md'));
  const parsed = remarkParser.parse(content);
  const tree = await rehypeConverter.run(parsed);
  console.log(JSON.stringify(parsed));
  const html = rehypeConverter.stringify(tree);

  return { html };
};
