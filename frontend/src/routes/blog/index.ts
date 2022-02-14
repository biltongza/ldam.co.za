import { process } from '$lib/blog/markdown';
import type { BlogMetadata } from '$lib/types';
import { DateFormat } from '$lib/__consts';
import type { RequestHandler } from '@sveltejs/kit';
import dayjs from 'dayjs';
import fs from 'fs';

export const get: RequestHandler = function () {
	const posts = fs
		.readdirSync(`src/posts`)
		.filter((fileName) => /.+\.md$/.test(fileName))
		.map((fileName) => {
			const { metadata } = process(`src/posts/${fileName}`);
			return metadata;
		});
	// sort the posts by create date.
	posts.sort((a, b) => dayjs(b.date, DateFormat).valueOf() - dayjs(a.date, DateFormat).valueOf());
	const result: Typify<BlogMetadata[]> = posts;

	return {
		body: {
			posts: result
		},
	};
};
