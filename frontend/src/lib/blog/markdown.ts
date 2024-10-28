import type { BlogMetadata } from '$lib/types';
import dayjs from 'dayjs';
import type { Nodes } from 'hast';
import yaml from 'js-yaml';
import type { Root } from 'mdast';
import rehypeExternalLinks from 'rehype-external-links';
import rehypeHighlight from 'rehype-highlight';
import rehypeStringify from 'rehype-stringify';
import rehypeUnwrapImages from 'rehype-unwrap-images';
import remarkFrontmatter from 'remark-frontmatter';
import remarkGfm from 'remark-gfm';
import remarkParse from 'remark-parse';
import remarkRehype from 'remark-rehype';
import { read } from 'to-vfile';
import { unified } from 'unified';

const remarkParser = unified().use(remarkParse).use(remarkGfm).use(remarkFrontmatter, ['yaml']);

const rehypeConverter = unified()
  .use(remarkRehype, {
    handlers: {
      heading: (state, node) => {
        const result: Nodes = {
          type: 'element',
          tagName: 'h' + (node.depth + 2),
          properties: {},
          children: state.all(node)
        };
        state.patch(node, result);
        return state.applyData(node, result);
      }
    }
  })
  .use(rehypeUnwrapImages)
  .use(rehypeHighlight)
  .use(rehypeExternalLinks, { rel: ['nofollow', 'noopener'] })
  .use(rehypeStringify);

export async function processMetadata(
  filename: string,
  log?: (...args: unknown[]) => void
): Promise<{ metadata: BlogMetadata; tree: Root }> {
  const start = Date.now();
  const file = await read(filename);
  const fileRead = Date.now();
  const remarkTree = remarkParser.parse(file);
  const parsed = Date.now();
  const slug = filename.slice(filename.lastIndexOf('/') + 1, -3);
  let metadata: BlogMetadata = null;
  if (remarkTree.children.length > 0 && remarkTree.children[0].type == 'yaml') {
    metadata = yaml.load(remarkTree.children[0].value) as BlogMetadata;
    remarkTree.children = remarkTree.children.slice(1, remarkTree.children.length);
    metadata.date = dayjs(metadata.date).valueOf();
    metadata.slug = slug;
  }
  if (!metadata) {
    throw new Error(`No Frontmatter in file ${filename}`);
  }
  const end = Date.now();
  log?.('markdown.ts processMetadata', {
    filename,
    fileRead: fileRead - start,
    parse: parsed - fileRead,
    total: end - start
  });
  return {
    metadata,
    tree: remarkTree
  };
}

export async function process(
  filename: string,
  log?: (...args: unknown[]) => void
): Promise<{ metadata: BlogMetadata; content: string }> {
  const start = Date.now();
  const { metadata, tree } = await processMetadata(filename, log);
  const parsed = Date.now();
  const rehypeTree = await rehypeConverter.run(tree);
  const converted = Date.now();
  const content = rehypeConverter.stringify(rehypeTree);
  const rendered = Date.now();
  log?.('markdown.ts process', {
    parse: parsed - start,
    convert: converted - parsed,
    render: rendered - converted,
    total: rendered - start
  });
  return { metadata, content };
}
