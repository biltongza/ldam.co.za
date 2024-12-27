<script lang="ts">
  import type { Album, ImageMetadata } from '$lib/types';
  import Thumbnail from './thumbnail.svelte';
  interface Props {
    album: Album;
    showHeader?: boolean;
    numberOfImages?: number;
  }

  let { album, showHeader = true, numberOfImages = Number.MAX_SAFE_INTEGER }: Props = $props();

  let entries = $derived(Object.entries(album.images).map(([, meta]) => meta));
  let images = $derived(
    entries
      .sort((meta1, meta2) => {
        const a = new Date(meta1.captureDate).valueOf();
        const b = new Date(meta2.captureDate).valueOf();

        return Number(b > a) - Number(b < a);
      })
      .slice(0, numberOfImages)
  );

  function getClassList(image: ImageMetadata): string {
    const parts = image.aspectRatio.split(':').map((part) => +part);
    const isLandscape = parts[0] > parts[1];
    return `thumbnail ${isLandscape ? 'span-col' : ''}`;
  }
</script>

<section class="album-container">
  {#if showHeader}
    <h2>{album.title}</h2>
  {/if}
  <div class="thumbnails">
    {#each images as image (image.id)}
      <div class={getClassList(image)}>
        <Thumbnail {image} />
      </div>
    {/each}
  </div>
</section>

<style>
  :root {
    --thumbnail-size: 480px;
  }
  .thumbnails {
    line-height: 0;
    display: grid;
    column-gap: var(--sl-spacing-small);
    row-gap: var(--sl-spacing-small);
    place-items: center;
    justify-content: center;
    grid-auto-flow: dense;
  }

  .thumbnail.span-col {
    grid-column: span 2;
  }

  .thumbnail {
    transition: all 0.1s ease-in-out;
  }

  .thumbnail:hover {
    transform: scale(1.02);
  }

  @media only screen and (max-width: 640px) {
    .thumbnail.span-col {
      grid-column: initial;
    }
  }

  @media only screen and (min-width: 768px) {
    .thumbnails {
      grid-template-columns: repeat(4, fit-content(var(--thumbnail-size)));
    }
  }

  @media only screen and (min-width: 1200px) {
    .thumbnails {
      grid-template-columns: repeat(6, fit-content(var(--thumbnail-size)));
    }
  }

  h2 {
    text-align: center;
  }
</style>
