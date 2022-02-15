import { getBlogPosts } from '$lib/getBlogPosts';
import { website } from '$lib/info';
import type { RequestHandler } from '@sveltejs/kit';

export const get: RequestHandler = async function () {
    const headers = {
        'Cache-Control': 'max-age=0, s-maxage=3600',
        'Content-Type': 'application/xml',
    }
    const posts = getBlogPosts();
	const rss = `<?xml version="1.0" encoding="UTF-8" ?>
    <rss xmlns:dc="https://purl.org/dc/elements/1.1/" 
            xmlns:content="https://purl.org/rss/1.0/modules/content/" 
            xmlns:atom="https://www.w3.org/2005/Atom" 
            version="2.0">
            <channel>
                <atom:link href="${website}/rss.xml" rel="self" type="application/rss+xml" />
                <title><![CDATA[Logan Dam]]></title>
                <link>${website}</link>
                <description><![CDATA[Logan Dam's blog on technology and life]]></description>
                <generator>SvelteKit</generator>
                <lastBuildDate>${new Date().toUTCString()}</lastBuildDate>
                <language>en</language>
                ${posts.map(post => 
                    `<item>
                    <title><![CDATA[${post.title}]]></title>
                    <description><![CDATA[${post.excerpt}]]></description>
                    <link>${website}/blog/${post.slug}</link>
                    <guid isPermaLink="false">${website}/blog/${post.slug}</guid>
                    <pubDate>${post.date}</pubDate>
                    ${post.tags.map(tag => `<category><![CDATA[${tag}]]></category>`).join('')}
                    </item>`).join('')}
            </channel>
            </rss>`;
	return {
        headers,
		body: rss,
	};
};
