<script lang="ts">
  import { base } from '$app/paths';
  import { DateFormat } from '$lib/__consts';
  import dayjs from 'dayjs';
  let { data } = $props();
</script>

<div>
  <p class="info">{data.posts.length} posts.</p>
  {#each data.posts as post}
    <div class="post">
      <a href={`${base}/blog/${post.slug}`}>
        <h2 class="title">{post.title}</h2>
      </a>
      <p class="date">
        <sl-icon name="calendar-event"></sl-icon>{dayjs(post.date).format(DateFormat)}
      </p>
      <p>
        {#each post.tags as tag}
          <sl-badge variant="primary" pill>{tag}</sl-badge>
        {/each}
      </p>
      <p>{post.excerpt}</p>
    </div>
  {/each}
</div>

<style lang="scss">
  .date {
    font-size: var(--sl-font-size-small);
    display: flex;
    flex-direction: row;
    gap: 0.5em;
    align-items: center;
  }
</style>
