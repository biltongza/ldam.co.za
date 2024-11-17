<script lang="ts">
  interface Props {
    text: string;
    label: string;
    icon: string;
    href: string;
  }

  let { text, label, icon, href }: Props = $props();
</script>

<a {href} target="_blank" rel="me" aria-label={label} class="url">
  <sl-icon name={icon} library="default"></sl-icon>
  <span>{text}</span>
</a>

<style lang="scss">
  a {
    line-height: 16px;
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 8px;
    &:hover {
      color: var(--sl-color-primary-600);
    }
    & > span {
      font-family: monospace;
      display: none;
      @media screen and (max-width: 550px) {
        display: unset;
      }
    }
  }
</style>
