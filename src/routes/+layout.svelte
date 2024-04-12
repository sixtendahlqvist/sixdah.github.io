<script>
	import "../app.postcss"
	import {AppShell} from "@skeletonlabs/skeleton";
</script>

<AppShell>
	<svelte:fragment slot="header">
		<div class="topBar">
			<nav class="navbar">
				<div class="logo">
					<div class="pfp"></div>
					<a class="header" href="/">Linus Scott</a>
				</div>
				<div class="menuItems">
					<div><a class="barItem" href="/Requests">Requests</a></div>
					<div><a class="barItem" href="/Info">About Me</a></div>
					<div><a class="barItem" href="/fileDownloads">Projects</a></div>
				</div>
			</nav>
		</div>
		<hr>
	</svelte:fragment>
	

	<slot />
</AppShell>

<style>
	.pfp{
		background-image: url(/pfp.jpg);
		background-size: contain;
		margin-left: 10px;
		width: 50px;
		height: 50px;
		border-radius: 50%;
	}
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
	.menuItems{
		grid-column: menu-start / menu-end;
		display: grid;
		grid-template-columns: 1fr 1fr 1fr;
		margin-top: 10px;
	}
	.logo{
		display: flex;
	}
</style>
