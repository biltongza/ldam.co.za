<script lang="ts">
	import { onMount } from 'svelte';
	import SocialIcon from './header/social-icon.svelte';
	import RouterLink from './router-link.svelte';
	let smol = false;
	let expanded = false;
	onMount(() => {
		const matchListener = (event: MediaQueryListEvent) => {
			smol = event.matches;
		};
		const mql = window.matchMedia('(max-width: 480px)');
		smol = mql.matches;
		mql.addEventListener('change', matchListener);
		return () => {
			mql.removeEventListener('change', matchListener);
		};
	});

	const routes = [
		{ label: 'About', path: '/about' },
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
		{ text: 'Twitter', label: 'twitter', href: 'https://twitter.com/TheBiltong', icon: 'twitter' }
	];
</script>

<header class="header">
	<div class="nav-container">
		<a class="brand" href="/"><h1>Logan Dam</h1></a>
		<nav class={smol && expanded ? 'expanded' : ''}>
			<ul>
				{#if smol}
					<li><RouterLink path="/">Home</RouterLink></li>
				{/if}
				{#each routes as { label, path }}
					<li><RouterLink {path}>{label}</RouterLink></li>
				{/each}
				{#each socials as { text, label, href, icon }}
					<li>
						<SocialIcon {text} {label} {href} {icon} showText={smol && expanded} />
					</li>
				{/each}
			</ul>
		</nav>
	</div>
	{#if smol}
		<sl-icon-button
			class={`menu-button ${expanded ? 'expanded' : ''}`}
			name="chevron-double-down"
			on:click={() => (expanded = !expanded)}
		/>
	{/if}
</header>

<style lang="scss">
	:root {
		--header-height: 60px;
	}

	h1 {
		text-transform: uppercase;
		font-size: 100%;
	}

	.header {
		padding-left: var(--sl-spacing-small);
		padding-right: var(--sl-spacing-small);
		background-color: var(--sl-color-neutral-100);
		margin-bottom: var(--sl-spacing-small);
		display: flex;
		flex-direction: row;
		align-items: start;

		@media only screen and (min-width: 1224px) {
			padding-left: var(--sl-spacing-2x-large);
			padding-right: var(--sl-spacing-2x-large);
		}
	}

	.nav-container {
		flex: 1 0 auto;
		display: flex;
		flex-direction: row;
		justify-content: space-between;
		align-items: center;
		@media screen and (max-width: 480px) {
			flex-direction: column;
			align-items: flex-start;
			font-size: var(--sl-font-size-large);
		}
	}

	.brand,
	.menu-button {
		margin-top: var(--sl-spacing-small);
		margin-bottom: var(--sl-spacing-small);
		font-size: var(--sl-font-size-x-large);
	}

	.menu-button {
		flex: 0 0 auto;
		margin-top: var(--sl-spacing-medium);
	}

	nav {
		@media screen and (max-width: 480px) {
			margin-bottom: var(--sl-spacing-medium);
			display: none;
			background-color: var(--sl-color-neutral-150);
			&.expanded {
				display: unset;
				width: 100%;
				ul {
					flex-direction: column;
					align-items: flex-start;
				}
			}
		}
		ul {
			list-style: none;
			display: flex;
			flex-direction: row;
			align-items: center;
			gap: var(--sl-spacing-medium);
			padding-inline-start: 0;
			margin: 0;
			li {
				min-height: 30px;
				display: flex;
				flex-direction: row;
				align-items: center;
			}
		}
	}
	.menu-button {
		display: none;
		@media only screen and (max-width: 480px) {
			display: block;
			transform: rotate(0deg);
			transition: all 0.25s ease-in;

			&.expanded {
				transform: rotate(180deg);
				transition: all 0.25s ease-out;
			}
		}
	}
</style>
