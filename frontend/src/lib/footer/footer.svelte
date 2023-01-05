<script>
	export const appver = __VERSION__;
	export const lastmod = __LASTMOD__;
</script>

<div class="footer">
	<p>
		made by <a href="https://github.com/biltongza/ldam.co.za" target="_blank" rel="noreferrer">me</a
		>
	</p>
	<div class="version"><span>{appver}</span>&nbsp;|<span>&nbsp;{lastmod}</span></div>
</div>

<style>
	.footer {
		padding-top: 2em;
		padding-bottom: 2em;
		display: flex;
		background-color: var(--sl-color-neutral-100);
		flex-direction: column;
		align-items: center;
        color: var(--sl-color-neutral-600);
        font-size: var(--sl-font-size-x-small);
        font-weight: var(--sl-font-weight-light);
	}
</style>
