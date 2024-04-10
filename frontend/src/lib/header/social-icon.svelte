<script lang="ts">
  export let text: string;
  export let label: string;
  export let icon: string;
  export let href: string;
</script>

<a {href} target="_blank" rel="me" aria-label={label}>
  <sl-icon name={icon} library="default" />
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
      font-family: 'Recursive', monospace;
      display: none;
      @media screen and (max-width: 550px) {
        display: unset;
      }
    }
  }
</style>
