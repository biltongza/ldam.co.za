<div class="header">
	<div class="brand"><a href="/"><h3>LOGAN DAM</h3></a></div>
	<div class="links">
        <nav>
            <a sveltekit:prefetch href="/about">About</a>
        </nav>
		<div class="socials">
			<sl-tooltip content="Github">
				<sl-icon-button
					name="github"
					target="_blank"
					label="github"
					href="https://github.com/biltongza"
				/>
			</sl-tooltip>
			<sl-tooltip content="LinkedIn">
				<sl-icon-button
					name="linkedin"
					target="_blank"
					label="linkedin"
					href="https://www.linkedin.com/in/logan-dam/"
				/>
			</sl-tooltip>
			<sl-tooltip content="Instagram">
				<sl-icon-button
					name="instagram"
					target="_blank"
					label="instagram"
					href="https://www.instagram.com/thebiltong/"
				/>
			</sl-tooltip>
			<sl-tooltip content="Twitter">
				<sl-icon-button
					name="twitter"
					target="_blank"
					label="twitter"
					href="https://twitter.com/TheBiltong"
				/>
			</sl-tooltip>
		</div>
	</div>
</div>

<style>
	.header {
		display: flex;
		justify-content: space-between;
		align-items: center;
	}

	a,
	a:hover,
	a:visited,
	a:active {
		color: inherit;
		text-decoration: none;
	}

	.links {
		display: flex;
		justify-content: flex-end;
		align-items: center;
	}
</style>