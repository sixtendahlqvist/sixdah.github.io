<script>
	import "../app.postcss"
	import {AppShell} from "@skeletonlabs/skeleton";
</script>

<AppShell>
	<svelte:fragment slot="header">
		<div class="topBar">
			<nav class="navbar">
				<div class="logo">
					<a class="header" href="/">Linus Scott</a>
				</div>
				<div class="menuItems">
					<div>
						<a class="barItem" href="/test">test</a>
					</div>
				</div>
			</nav>
		</div>
		<hr>
		<hr>
	</svelte:fragment>


	<slot />
</AppShell>

<style>
	.header{
		margin: 15px;
		text-align: left;
		font-size: xx-large;
		font-weight: bold;
	}
	.navbar{
		display: grid;
		grid-template-columns: [logo-start] 1fr [logo-end] 1fr [menu-start] 1fr [menu-end];
	}
	.menuItems{
		grid-column: menu-start / menu-end;
	}
</style>
