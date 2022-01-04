<script>
	import RouterLink from './router-link.svelte';
</script>

<div class="header">
	<div class="brand"><a href="/"><h1>Logan Dam</h1></a></div>
	<div class="links">
		<nav>
			<RouterLink path="/about">About</RouterLink>
		</nav>
		<div class="socials">
			<sl-icon-button
				name="github"
				target="_blank"
				label="github"
				href="https://github.com/biltongza"
			/>
			<sl-icon-button
				name="linkedin"
				target="_blank"
				label="linkedin"
				href="https://www.linkedin.com/in/logan-dam/"
			/>
			<sl-icon-button
				name="instagram"
				target="_blank"
				label="instagram"
				href="https://www.instagram.com/thebiltong/"
			/>
			<sl-icon-button
				name="twitter"
				target="_blank"
				label="twitter"
				href="https://twitter.com/TheBiltong"
			/>
		</div>
	</div>
</div>

<style>
	.header {
		display: flex;
		justify-content: space-between;
		align-items: center;
		height: 60px;
		background-color: var(--sl-color-neutral-100);
	}

	.links {
		display: flex;
		justify-content: flex-end;
		align-items: center;
	}

	h1 {
		text-transform: uppercase;
		font-size: 100%;
	}

	.header {
		padding-left: var(--sl-spacing-small);
		padding-right: var(--sl-spacing-small);
	}

	@media only screen and (min-width: 1224px) {
		.header {
			padding-left: var(--sl-spacing-2x-large);
			padding-right: var(--sl-spacing-2x-large);
		}
	}
</style>
