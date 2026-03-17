<script>
  export const appver = __VERSION__;
  export const lastmod = __LASTMOD__;
</script>

<div class="footer">
  <div>
    made by <a
      href="https://github.com/biltongza/ldam.co.za"
      target="_blank"
      rel="nofollow noopener">me</a
    >
  </div>
  <div class="webring">
    <a href="https://webring.nabeelvalley.co.za"><em>Webring</em></a>

    <ul class="links">
        <li><a href="https://webring.nabeelvalley.co.za/previous" class="svelte-ol6up6">Previous</a></li>
        <li><a href="https://webring.nabeelvalley.co.za/random" class="svelte-ol6up6">Random</a></li>
        <li><a href="https://webring.nabeelvalley.co.za/next" class="svelte-ol6up6">Next</a></li>
    </ul>
  </div>
  <div class="version"><span>{appver}</span>&nbsp;|<span>&nbsp;{lastmod}</span></div>
</div>

<style>
  .footer {
    padding-top: 2em;
    display: flex;
    background-color: var(--sl-color-neutral-100);
    flex-direction: column;
    align-items: center;
    color: var(--sl-color-neutral-600);
    font-size: var(--sl-font-size-x-small);
    font-weight: var(--sl-font-weight-light);
  }
  .footer * {
    padding-bottom: 2em;
  }
  .webring a {
    display: inline;
  }
  .webring .links {
    display: inline-flex;
    padding-left: 0;
    gap: 0.5em;
    list-style: none;
  }
</style>
