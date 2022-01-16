import { metadata } from '$lib/.metadata.js';
import { website } from '$lib/info';
import type { ImageMetadata, Manifest } from '$lib/types';
import { StorageBaseUrl } from '$lib/__consts';
import type { EndpointOutput } from '@sveltejs/kit';
import type { DefaultBody } from '@sveltejs/kit/types/endpoint';
export async function get(): Promise<EndpointOutput<DefaultBody>> {
	const headers = {
		'Cache-Control': 'max-age=0, s-maxage=3600',
		'Content-Type': 'application/xml'
	};
	const response = await fetch(`${StorageBaseUrl}/manifest.json?t=${new Date().valueOf()}`);
  if(!response.ok) {
    throw new Error(`failed to load manifest: ${response.status}`);
  }
	const manifest = (await response.json()) as Manifest;
	const routes = { about: 'src/routes/about.svelte' };
	const files = Object.entries(routes).map(([route, path]) => {
		const lastModified = metadata.find((x) => x.path === path).lastModified;
		return { route, lastModified: new Date(lastModified).toISOString() };
	});

	const imageRouteLastModified = metadata.find(
		(x) => x.path === 'src/routes/image/[imageId].svelte'
	).lastModified;

	function generateImageNode(metadata: ImageMetadata): string {
		return `<url>
    <loc>${website}/image/${metadata.id}</loc>
    <image:image>
      <image:loc>${StorageBaseUrl}/${metadata.id}.2048.jpg</image:loc>
      <image:title>${metadata.title}</image:title>
      <image:license>http://creativecommons.org/licenses/by-nc/4.0</image:license>
    </image:image>
    <lastmod>${new Date(
			Math.max.apply(null, [imageRouteLastModified, new Date(metadata.lastModified)])
		).toISOString()}</lastmod>
  </url>`;
	}

	return {
		headers,
		body: `<?xml version="1.0" encoding="UTF-8" ?>
      <urlset
        xmlns="https://www.sitemaps.org/schemas/sitemap/0.9"
        xmlns:xhtml="https://www.w3.org/1999/xhtml"
        xmlns:image="https://www.google.com/schemas/sitemap-image/1.1"
      >
      <url>
        <loc>${website}</loc>
        <lastmod>${new Date(manifest.lastModified).toISOString()}</lastmod>
      </url>
      ${files
				.map(
					({ route, lastModified }) => `
        <url>
          <loc>${website}/${route}</loc>
          <lastmod>${lastModified}</lastmod>
        </url>`
				)
				.join('')}
      ${Object.entries(manifest.albums)
				.flatMap(([, album]) => {
					return Object.entries(album.images).map(([, metadata]) => generateImageNode(metadata));
				})
				.join('')}
      </urlset>`
	};
}
