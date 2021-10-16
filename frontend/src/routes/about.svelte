<script context="module">
    export const prerender = true;
</script>
<script>
	let yearsInField = new Date().getFullYear() - 2013;
</script>

<div class="content">
	<div class="blurb">
		<h3>Hi. I'm Logan.</h3>
		<p>
			I'm a full time software engineer, with {yearsInField} years in the industry, with a strong technical
			background in .NET and MSSQL. I have a passion for helping others realise their potential and enjoy
			mentorship and elevating the abilities of my peers.
		</p>
		<p>
			I'm a hobbyist photographer in my free time. I enjoy capturing depth, colour, and detail, and
			frequently use my photography to express my emotions.
		</p>
	</div>

	<img src="/Selfie.jpg" class="selfie" />
</div>

<style>
	.content {
		max-width: 900px;
		margin-left: auto;
		margin-right: auto;
		display: flex;
		row-gap: 1rem;
		flex-direction: column;
	}
	@media only screen and (min-width: 1224px) {
		.content {
			flex-direction: row;
		}
	}

	.selfie {
		max-width: 400px;
	}
</style>
