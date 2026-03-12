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
      <a href="https://webring.nabeelvalley.co.za/previous">Previous</a>
      <a href="https://webring.nabeelvalley.co.za/random">Random</a>
      <a href="https://webring.nabeelvalley.co.za/next">Next</a>
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
    display: inline;
    padding-left: 0;
  }
</style>
