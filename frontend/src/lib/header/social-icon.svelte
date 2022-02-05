<script lang="ts">
	export let showText = false;
	export let text: string;
	export let label: string;
	export let icon: string;
	export let href: string;
</script>

<sl-button variant="default" size="medium" {href} target="_blank" {label}>
	<sl-icon slot="prefix" name={icon} />
	{showText ? text : ''}
</sl-button>

<style lang="scss">
	sl-button::part(base) {
		border-radius: unset;
		background-color: unset;
		color: unset;
		border: none;

		padding: unset;
		&:hover {
			color: var(--sl-color-primary-600);
		}
	}
	sl-button::part(label) {
		font-family: 'Recursive', monospace;
		line-height: unset;
	}
</style>
