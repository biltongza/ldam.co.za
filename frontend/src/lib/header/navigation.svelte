<script lang="ts">
  import RouterLink from './router-link.svelte';
  import SocialIcon from './social-icon.svelte';

  const routes = [
    { label: 'About', path: '/about' },
    { label: 'Collections', path: '/collections' },
    { label: 'Blog', path: '/blog' }
  ];

  const socials = [
    { text: 'GitHub', label: 'github', href: 'https://github.com/biltongza', icon: 'github' },
    {
      text: 'LinkedIn',
      label: 'linkedin',
      href: 'https://www.linkedin.com/in/logan-dam/',
      icon: 'linkedin'
    },
    {
      text: 'Instagram',
      label: 'instagram',
      href: 'https://www.instagram.com/thebiltong/',
      icon: 'instagram'
    },
    { text: 'Twitter', label: 'twitter', href: 'https://twitter.com/TheBiltong', icon: 'twitter-x' }
  ];
  let expanded = false;
</script>

<nav>
  <ul class={expanded ? 'expanded' : ''}>
    <li><RouterLink path="/">Home</RouterLink></li>
    {#each routes as { label, path }}
      <li><RouterLink {path}>{label}</RouterLink></li>
    {/each}
    {#each socials as { text, label, href, icon }}
      <li>
        <SocialIcon {text} {label} {href} {icon} />
      </li>
    {/each}
  </ul>
  <sl-icon-button
    class={`menu-button ${expanded ? 'expanded' : ''}`}
    name="chevron-double-down"
    on:click={() => (expanded = !expanded)}
  />
</nav>

<style lang="scss">
  nav {
    display: flex;
    flex-direction: row;
    align-items: flex-start;
    margin-inline-end: var(--sl-spacing-medium);
    padding-top: var(--sl-spacing-large);

    ul {
      list-style: none;
      display: flex;
      flex-direction: row;
      align-items: baseline;
      gap: var(--sl-spacing-medium);
      padding-inline-start: 0;
      margin: 0;

      @media screen and (max-width: 550px) {
        height: 0;
        overflow: hidden;
        transition: height 0.5s ease-in;
        flex-direction: column;
        margin-top: 32px;
        margin-bottom: 32px;
        &.expanded {
          height: 300px;
          transition: height 0.5s ease-out;
        }
      }
      li {
        display: flex;
        flex-direction: row;
        align-items: center;
      }
    }

    .menu-button {
      display: none;
      transform: rotate(0deg);
      transition: all 0.5s ease-in;
      @media screen and (max-width: 550px) {
        display: unset;
        flex: none;
      }

      &.expanded {
        transform: rotate(180deg);
        transition: all 0.5s ease-out;
      }
    }
  }
</style>
