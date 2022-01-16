import { process } from '$lib/markdown';
import { DateFormat } from '$lib/__consts';
import type { RequestHandler } from '@sveltejs/kit';
import dayjs from 'dayjs';
import fs from 'fs';

const get: RequestHandler = function () {
	const posts = fs
		.readdirSync(`src/posts`)
		.filter((fileName) => /.+\.md$/.test(fileName))
		.map((fileName) => {
			const { metadata } = process(`src/posts/${fileName}`);
			return metadata;
		});
	// sort the posts by create date.
	posts.sort(
		(a, b) =>
			dayjs(a.date, DateFormat).valueOf() - dayjs(b.date, DateFormat).valueOf()
	);
	const body = JSON.stringify(posts);

	return {
		body
	};
};

export { get };