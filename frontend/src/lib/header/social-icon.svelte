<script lang="ts">
  export let text: string;
  export let label: string;
  export let icon: string;
  export let href: string;
</script>

<sl-button variant="default" {href} target="_blank" {label}>
  <sl-icon slot="prefix" name={icon} library="default" />
  {text}
</sl-button>

<style lang="scss">
  sl-button {
    &::part(base) {
      border-radius: unset;
      background-color: unset;
      color: unset;
      border: none;
      padding: unset;
      min-height: unset;
      line-height: unset;
      display: flex;
      flex-direction: row;
      align-items: center;
      //font-size: 16px;

      &:hover {
        color: var(--sl-color-primary-600);
      }
    }
  }

  sl-button::part(label) {
    font-family: 'Recursive', monospace;
    line-height: unset;
    display: none;
    @media screen and (max-width: 550px) {
      display: unset;
    }
  }
</style>
