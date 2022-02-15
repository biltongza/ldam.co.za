import { process } from '$lib/blog/markdown';
import fs from 'fs';


export const getBlogPosts = function () {
    const posts = fs
		.readdirSync(`src/posts`)
		.filter((fileName) => /.+\.md$/.test(fileName))
		.map((fileName) => {
			const { metadata } = process(`src/posts/${fileName}`);
			return metadata;
		});
	// sort the posts by create date.
	posts.sort((a, b) => b.date.valueOf() - a.date.valueOf());

    return posts;
}