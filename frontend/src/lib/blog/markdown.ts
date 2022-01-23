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
import type { BlogMetadata } from '../types';
import { DateFormat } from '../__consts';

const parser = unified().use(parse).use(gfm).use(frontmatter, ['yaml']);

const runner = unified()
	.use(remark2rehype)
	.use(highlight)
	.use(rehypeExternalLinks, { target: false, rel: ['nofollow', 'noopener'] })
	.use(rehypeStringify)
	.use(remarkUnwrapImages);

export function process(filename: string): { metadata: BlogMetadata; content: string } {
	const tree = parser.parse(vfile.readSync(filename));
	let metadata: BlogMetadata = null;
	const slug = filename.slice(filename.lastIndexOf('/') + 1, -3);
	if (tree.children.length > 0 && tree.children[0].type == 'yaml') {
		metadata = yaml.load(tree.children[0].value) as BlogMetadata;
		tree.children = tree.children.slice(1, tree.children.length);
		metadata.date = dayjs(metadata.date).format(DateFormat);
		metadata.slug = slug;
	}
	let content = runner.stringify(runner.runSync(tree));
	if (!metadata) {
		metadata = {
			title: 'Error!',
			date: '?',
			excerpt: 'Missing Frontmatter! Expected at least a title and a date!',
			slug: slug,
			tags: [],
		};
		content = 'Missing Frontmatter! Expected at least a title and a date!';
	}
	return { metadata, content };
}
