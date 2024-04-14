import type { BlogMetadata } from '$lib/types';
import dayjs from 'dayjs';
import type { Nodes } from 'hast';
import yaml from 'js-yaml';
import type { Root } from 'mdast';
import rehypeExternalLinks from 'rehype-external-links';
import rehypeHighlight from 'rehype-highlight';
import rehypeStringify from 'rehype-stringify';
import remarkFrontmatter from 'remark-frontmatter';
import remarkGfm from 'remark-gfm';
import remarkParse from 'remark-parse';
import remarkRehype from 'remark-rehype';
import remarkUnwrapImages from 'remark-unwrap-images';
import * as vfile from 'to-vfile';
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
  .use(remarkUnwrapImages)
  .use(rehypeHighlight)
  .use(rehypeExternalLinks, { rel: ['nofollow', 'noopener'] })
  .use(rehypeStringify);

export async function processMetadata(
  filename: string
): Promise<{ metadata: BlogMetadata; tree: Root }> {
  const file = await vfile.read(filename);
  const remarkTree = remarkParser.parse(file);
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
  return {
    metadata,
    tree: remarkTree
  };
}

export async function process(
  filename: string
): Promise<{ metadata: BlogMetadata; content: string }> {
  const { metadata, tree } = await processMetadata(filename);
  const rehypeTree = await rehypeConverter.run(tree);
  const content = rehypeConverter.stringify(rehypeTree);
  return { metadata, content };
}
