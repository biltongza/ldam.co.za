import { StorageBaseUrl } from '$lib/__consts';
import type { Manifest } from '$lib/types';

export async function getManifest(fetchFn?: (id: string) => Promise<Response>): Promise<Manifest> {
  const f = fetchFn || fetch;
  const response = await f(`${StorageBaseUrl}/manifest.json`);
  if (!response.ok) {
    throw new Error(`failed to load manifest: ${response.status}`);
  }
  const manifest = (await response.json()) as Manifest;

  return manifest;
}
