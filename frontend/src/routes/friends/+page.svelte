<script lang="ts">
  import { FriendSites } from '$lib/__consts';
</script>

Here are some friends of mine:
<ul>
  {#each FriendSites as site (site.title)}
    <li>
      <div>
        <a href={site.domain} title={site.title}>{site.name}</a>
      </div>
      <div>
        {site.blurb}
      </div>
    </li>
  {/each}
</ul>

<style>
  li {
    margin-bottom: 1em;
  }
</style>
