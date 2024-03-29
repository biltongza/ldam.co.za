import type { BlogMetadata } from '$lib/types';
import dayjs from 'dayjs';
import yaml from 'js-yaml';
import rehypeExternalLinks from 'rehype-external-links';
import highlight from 'rehype-highlight';
import rehypeStringify from 'rehype-stringify';
import frontmatter from 'remark-frontmatter';
import gfm from 'remark-gfm';
import parse from 'remark-parse';
import remark2rehype from 'remark-rehype';
import remarkUnwrapImages from 'remark-unwrap-images';
import * as vfile from 'to-vfile';
import { unified } from 'unified';

const parser = unified().use(parse).use(gfm).use(frontmatter, ['yaml']);

const runner = unified()
	.use(remark2rehype)
	.use(highlight)
	.use(rehypeExternalLinks, { rel: ['nofollow', 'noopener'] })
	.use(rehypeStringify)
	.use(remarkUnwrapImages);

export function process(filename: string): { metadata: BlogMetadata; content: string } {
	const tree = parser.parse(vfile.readSync(filename));
	let metadata: BlogMetadata = null;
	const slug = filename.slice(filename.lastIndexOf('/') + 1, -3);
	if (tree.children.length > 0 && tree.children[0].type == 'yaml') {
		metadata = yaml.load(tree.children[0].value) as BlogMetadata;
		tree.children = tree.children.slice(1, tree.children.length);
		metadata.date = dayjs(metadata.date).valueOf();
		metadata.slug = slug;
	}

	// the typings of remark and rehype are not compatible but remark2rehype works just fine anyway
	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	const remarkTree: any = runner.runSync(tree)
	let content = runner.stringify(remarkTree);
	if (!metadata) {
		metadata = {
			title: 'Error!',
			date: dayjs().valueOf(),
			excerpt: 'Missing Frontmatter! Expected at least a title and a date!',
			slug: slug,
			tags: []
		};
		content = 'Missing Frontmatter! Expected at least a title and a date!';
	}
	return { metadata, content };
}
