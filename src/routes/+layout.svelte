<script>
	import "../app.postcss"
	import {AppShell} from "@skeletonlabs/skeleton";
</script>


<!--Linus kod-->
<AppShell>
	<svelte:fragment slot="header">
		<div class="topBar">
			<nav class="navbar">
				<div class="logo">
					<a class="header" href="/">Sixten Dahlqvist</a>
				</div>
			</nav>
		</div>
		<hr>
	</svelte:fragment>
	

	<slot />
</AppShell>

<style>
	.header{
		margin: 10px;
		margin-bottom: 20px;
		text-align: left;
		font-size: xx-large;
		font-weight: bold;
	}
	.navbar{
		margin-top: 10px;
		display: grid;
		grid-template-columns: [logo-start] 1fr [logo-end] 1fr [menu-start] 1fr [menu-end];
	}
</style>
