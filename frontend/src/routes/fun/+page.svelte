<script>
  import RouterLink from '$lib/header/router-link.svelte';
</script>

<ul class="fun-list">
  <li>
    <RouterLink path="/fun/carriage"><h2>Carriage Calculator</h2></RouterLink>
    <p>A little game to keep your brain busy on the London tube.</p>
  </li>
</ul>

<style>
  .fun-list {
    list-style: none;
  }
</style>
