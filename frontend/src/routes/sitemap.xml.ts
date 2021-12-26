import { website } from '$lib/info';
import type { Manifest } from '$lib/manifest';
import { StorageBaseUrl } from '$lib/__consts';
import type { EndpointOutput } from '@sveltejs/kit';
import type { DefaultBody } from '@sveltejs/kit/types/endpoint';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export async function get(): Promise<EndpointOutput<DefaultBody>> {
	const headers = {
		'Cache-Control': 'max-age=0, s-maxage=3600',
		'Content-Type': 'application/xml'
	};
	const response = await fetch(`${StorageBaseUrl}/manifest.json?t=${new Date().valueOf()}`);
	const manifest = (await response.json()) as Manifest;
	const routes = ['about'];
	const files = routes.map((route) => ({ route, stat: fs.statSync(`${path.join(__dirname, route)}.svelte`) }));

	return {
		headers,
		body: `<?xml version="1.0" encoding="UTF-8" ?>
      <urlset
        xmlns="https://www.sitemaps.org/schemas/sitemap/0.9"
        xmlns:news="https://www.google.com/schemas/sitemap-news/0.9"
        xmlns:xhtml="https://www.w3.org/1999/xhtml"
        xmlns:mobile="https://www.google.com/schemas/sitemap-mobile/1.0"
        xmlns:image="https://www.google.com/schemas/sitemap-image/1.1"
        xmlns:video="https://www.google.com/schemas/sitemap-video/1.1"
      >
      <url>
        <loc>${website}</loc>
        <lastmod>${manifest.lastModified}</lastmod>
      </url>
      ${
        files.map(({route, stat}) => `
        <url>
          <loc>${website}/${route}</loc>
          <lastmod>${stat.mtime.toISOString()}</lastmod>
        </url>`).join('')}
      ${Object.entries(manifest.albums)
				.flatMap(([, album]) => {
					return Object.entries(album.images).map(
						([key, metadata]) => `
        <url>
          <loc>${website}/image/${key}</loc>
          <lastmod>${metadata.lastModified}</lastmod>
        </url>`
					);
				})
				.join('')}
      </urlset>`
	};
}
