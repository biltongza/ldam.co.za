<script lang="ts">
  import { StorageBaseUrl, ThumbnailHrefNormalDensity } from '$lib/__consts';
  import type { ImageMetadata } from '$lib/types';

  const thumbnailSizes = {
    thumbnail2x: '320w',
    '640': '640w',
    '1280': '1280w'
  };

  // order is important here apparently
  const formats = {
    webp: 'image/webp',
    jpg: 'image/jpeg'
  };

  interface Props {
    image: ImageMetadata;
  }

  let { image }: Props = $props();

  let imageRoute: string = $derived(`/image/${image.id}`);
  let src: string = $derived(`${StorageBaseUrl}/${image.hrefs[ThumbnailHrefNormalDensity]}.webp`);
  let srcSets = $derived(
    Object.entries(thumbnailSizes).flatMap(([sizeKey, maxWidth]) =>
      Object.entries(formats).map(([extension, mime]) => ({
        srcSet: `${StorageBaseUrl}/${image.hrefs[sizeKey]}.${extension} ${maxWidth}`,
        mime
      }))
    )
  );
</script>

<a href={imageRoute}>
  <picture>
    {#each srcSets as srcset}
      <source srcset={srcset.srcSet} type={srcset.mime} />
    {/each}
    <img {src} class="thumbnail-image" loading="lazy" alt="" />
  </picture>
</a>

<style>
  a {
    display: block;
    width: 100%;
    height: 100%;
  }
  .thumbnail-image {
    width: 100%;
    height: 100%;
    object-fit: contain;
    max-height: 85vh;
    background-image: url('/tail-spin.svg');
    background-repeat: no-repeat;
    background-position: center;
  }
</style>
