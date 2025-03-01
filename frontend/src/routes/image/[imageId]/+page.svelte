<script lang="ts">
  import { HighResHref, HighResMaxDimension, StorageBaseUrl } from '$lib/__consts.js';
  import { usePageMetadata, useTitle } from '$lib/stores.svelte.js';
  import { onMount } from 'svelte';
  let { data } = $props();
  let { metadata } = $derived(data);

  const title = useTitle();
  const pageMetadata = usePageMetadata();

  const href = $derived(metadata.hrefs[HighResHref]);
  const src = $derived(`${StorageBaseUrl}/${href}.jpg`);

  const date = $derived(new Date(metadata.captureDate));

  onMount(() => {
    title.value = metadata.title;

    const [widthRatio, heightRatio] = metadata.aspectRatio.split(':').map((x) => Number(x));
    const width =
      widthRatio > heightRatio
        ? HighResMaxDimension
        : (widthRatio / heightRatio) * HighResMaxDimension;
    const height =
      heightRatio > widthRatio
        ? HighResMaxDimension
        : (heightRatio / widthRatio) * HighResMaxDimension;
    pageMetadata.set({
      'twitter:card': 'summary_large_image',
      'og:type': 'article',
      'og:image': src,
      'og:title': metadata.title,
      'og:description': 'Photography by Logan Dam',
      'og:image:width': width.toString(),
      'og:image:height': height.toString(),
      'og:image:alt': metadata.caption
    });

    return () => {
      title.value = undefined;
      pageMetadata.clear();
    };
  });
</script>

<article class="image-container">
  <img {src} class="image" alt={metadata.caption} />
  <section class="metadata">
    {#if metadata.title}
      <h2 class="title">{metadata.title}</h2>
    {/if}
    {#if metadata.caption}
      <h3 class="caption">{metadata.caption}</h3>
    {/if}
    <div class="date-info">
      <sl-icon name="calendar-event"></sl-icon>
      <div class="content">
        <time datetime={date.toISOString()}>{date.toLocaleDateString()}</time>
      </div>
    </div>
    <div class="camera-info">
      <sl-icon library="ionicons" name="camera-outline"></sl-icon>
      <div class="content">
        <div>{metadata.cameraModel}</div>
        <div>{metadata.lens}</div>
      </div>
    </div>
    <div class="exposure-info">
      <sl-icon library="ionicons" name="aperture-outline"></sl-icon>
      <div class="content">
        <span>ISO {metadata.iso}</span>
        <span>{metadata.focalLength}</span>
        <span>{metadata.fNumber}</span>
        <span>{metadata.shutterSpeed}</span>
      </div>
    </div>
    <div>
      This work is licensed under <a
        href="http://creativecommons.org/licenses/by-nc/4.0/?ref=chooser-v1"
        target="_blank"
        rel="license noopener noreferrer"
        style="display:inline-block;"
        >CC BY-NC 4.0<img
          style="height:22px!important;margin-left:3px;vertical-align:text-bottom;"
          src="https://mirrors.creativecommons.org/presskit/icons/cc.svg?ref=chooser-v1"
          alt="cc"
        /><img
          style="height:22px!important;margin-left:3px;vertical-align:text-bottom;"
          src="https://mirrors.creativecommons.org/presskit/icons/by.svg?ref=chooser-v1"
          alt="by"
        /><img
          style="height:22px!important;margin-left:3px;vertical-align:text-bottom;"
          src="https://mirrors.creativecommons.org/presskit/icons/nc.svg?ref=chooser-v1"
          alt="nc"
        /></a
      >
    </div>
  </section>
</article>

<style>
  .image {
    object-fit: contain;
    justify-content: center;
    background-image: url('/tail-spin.svg');
    background-repeat: no-repeat;
    background-position: center;
    height: 100%;
    width: 100%;
    max-height: 85vh;
  }

  .metadata {
    flex: 1 1 auto;
  }

  .image-container {
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 2em;
  }
  @media only screen and (min-width: 768px) {
    .image-container {
      flex-direction: row;
      height: 90vh;
    }
  }

  .metadata {
    display: flex;
    flex-direction: column;
    gap: 1em;
  }

  .metadata > * > sl-icon {
    font-size: var(--sl-font-size-large);
  }

  .camera-info,
  .exposure-info,
  .date-info {
    display: flex;
    flex-direction: row;
    align-items: flex-start;
    justify-content: start;
    gap: 1em;
  }

  .exposure-info > .content {
    display: flex;
    flex-direction: row;
    justify-content: space-evenly;
    gap: 1em;
  }
</style>
