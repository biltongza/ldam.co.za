<script>
	import RouterLink from './router-link.svelte';
</script>

<div class="header">
	<div class="brand"><a href="/"><h3>LOGAN DAM</h3></a></div>
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
	}

	.links {
		display: flex;
		justify-content: flex-end;
		align-items: center;
	}
</style>
